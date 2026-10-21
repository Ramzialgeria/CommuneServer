using System;
using System.Configuration;
using System.IO;
using System.Linq;
using CommuneSecurity.Activation;
using Newtonsoft.Json;
using CommuneServer.Data.Entities;
using CommuneServer.Data;
using System.Data.Entity;

namespace CommuneServer.Activation
{
    public class ActivationService
    {
        public bool Activated(CommuneDbContext db)
        {
            return db.Activations.Any(x => x.IsActive);
        }

        public ActivationResult Activate(
            string filePath,
            string activationCode,
            CommuneDbContext db)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return ActivationResult.Fail(
                    "Activation file was not specified.");

            if (!File.Exists(filePath))
                return ActivationResult.Fail(
                    "Activation file does not exist.");

            try
            {
                // 1) signature check (issued by central, not modified)
                // 2) decryption with the activation code
                byte[] fileBytes = File.ReadAllBytes(filePath);

                System.Diagnostics.Debug.WriteLine(
                    $"File: {filePath}");

                System.Diagnostics.Debug.WriteLine(
                    $"File size: {fileBytes.Length}");

                System.Diagnostics.Debug.WriteLine(
                    $"Code length: {(activationCode ?? "").Length}");

  

                var json = ActivationFileReader.Open(
                    File.ReadAllBytes(filePath),
                    activationCode,
                    CentralPublicKey.Xml);

                var package =
                    JsonConvert.DeserializeObject<ActivationPackage>(json);

                if (package == null)
                    return ActivationResult.Fail(
                        "Invalid activation file.");

                Validate(package);

                // Already activated?
                if (db.Activations.Any(x => x.IsActive))
                {
                    return ActivationResult.Fail(
                        "This installation is already activated.");
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        ImportCommune(package, db);
                        ImportListes(package, db);
                        ImportUsers(package, db);

                        var activation = new ActivationInfo
                        {
                            ActivationId = package.ActivationId,

                            ActivationVersion =
                                package.ActivationVersion,

                            CodeCommune =
                                package.ActivationType == "APW"
                                    ? package.Wilaya.CodeWilaya
                                    : package.Commune.CodeCommune,

                            ActivatedAt = DateTime.UtcNow,

                            IsActive = true
                        };

                        db.Activations.Add(activation);

                        db.SaveChanges();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

                return ActivationResult.Success(
                    package.ActivationType == "APW"
                        ? package.Wilaya.CodeWilaya
                        : package.Commune.CodeCommune,

                    package.ActivationType == "APW"
                        ? package.Wilaya.NameWilaya
                        : package.Commune.NameCommune);
            }
            catch (Exception ex)
            {
                return ActivationResult.Fail(ex.Message);
            }
        }


        private void Validate(ActivationPackage package)
        {
            if (package == null)
                throw new Exception(
                    "Activation package is empty.");

            if (package.ActivationVersion <= 0)
                throw new Exception(
                    "Invalid activation version.");

            if (string.IsNullOrWhiteSpace(package.ActivationId))
                throw new Exception(
                    "ActivationId is missing.");

            if (string.IsNullOrWhiteSpace(package.ActivationType))
                throw new Exception(
                    "ActivationType is missing.");

            // The file is made for ONE installation
            var serverCode = ConfigurationManager.AppSettings["ServerCode"];

            if (string.IsNullOrWhiteSpace(package.ServerCode) ||
                !package.ServerCode.Trim().Equals(
                    (serverCode ?? "").Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "This activation file was issued for another server.");
            }

            if (package.ActivationType.Equals(
                "APW",
                StringComparison.OrdinalIgnoreCase))
            {
                ValidateAPW(package);
            }
            else if (package.ActivationType.Equals(
                "APC",
                StringComparison.OrdinalIgnoreCase))
            {
                ValidateAPC(package);
            }
            else
            {
                throw new Exception(
                    "Invalid activation type: " +
                    package.ActivationType);
            }

            ValidateListes(package);
            ValidateUsers(package);
        }

        private void ValidateAPW(ActivationPackage package)
        {
            if (package.Wilaya == null)
                throw new Exception(
                    "Wilaya information is missing.");

            if (string.IsNullOrWhiteSpace(
                package.Wilaya.CodeWilaya))
                throw new Exception(
                    "Wilaya code is missing.");

            if (string.IsNullOrWhiteSpace(
                package.Wilaya.NameWilaya))
                throw new Exception(
                    "Wilaya name is missing.");

            if (package.Communes == null ||
                package.Communes.Count == 0)
                throw new Exception(
                    "Communes are missing.");

            if (package.Commune != null)
                throw new Exception(
                    "APW activation cannot contain a single commune.");

            foreach (var commune in package.Communes)
            {
                ValidateCommune(commune);
            }
        }

        private void ValidateAPC(ActivationPackage package)
        {
            if (package.Commune == null)
                throw new Exception(
                    "Commune information is missing.");

            if (package.Wilaya != null)
                throw new Exception(
                    "APC activation cannot contain Wilaya information.");

            if (package.Communes != null &&
                package.Communes.Count > 0)
                throw new Exception(
                    "APC activation cannot contain multiple communes.");

            ValidateCommune(package.Commune);
        }

        private void ValidateCommune(ActivationCommune commune)
        {
            if (commune == null)
                throw new Exception(
                    "Invalid commune.");

            if (string.IsNullOrWhiteSpace(
                commune.CodeCommune))
                throw new Exception(
                    "Commune code is missing.");

            if (string.IsNullOrWhiteSpace(
                commune.NameCommune))
                throw new Exception(
                    "Commune name is missing.");

            if (string.IsNullOrWhiteSpace(
                commune.CodeWilaya))
                throw new Exception(
                    "Commune Wilaya code is missing.");

            if (commune.Centres == null ||
                commune.Centres.Count == 0)
                throw new Exception(
                    "No centres found for commune " +
                    commune.CodeCommune);

            foreach (var centre in commune.Centres)
            {
                if (centre == null)
                    throw new Exception(
                        "Invalid centre.");

                if (centre.Id <= 0)
                    throw new Exception(
                        "Invalid centre ID.");

                if (string.IsNullOrWhiteSpace(centre.Code))
                    throw new Exception(
                        "Centre code is missing.");

                if (centre.Bureaux == null ||
                    centre.Bureaux.Count == 0)
                    throw new Exception(
                        "No bureaux found for centre " +
                        centre.Code);

                foreach (var bureau in centre.Bureaux)
                {
                    if (bureau == null)
                        throw new Exception(
                            "Invalid bureau.");

                    if (bureau.Id <= 0)
                        throw new Exception(
                            "Invalid bureau ID.");

                    if (string.IsNullOrWhiteSpace(bureau.Code))
                        throw new Exception(
                            "Bureau code is missing.");
                }
            }
        }

        private void ValidateListes(ActivationPackage package)
        {
            if (package.Listes == null ||
                package.Listes.Count == 0)
                throw new Exception(
                    "No electoral lists found in activation file.");

            foreach (var liste in package.Listes)
            {
                if (liste == null)
                    throw new Exception(
                        "Invalid electoral list.");

                if (liste.Id <= 0)
                    throw new Exception(
                        "Invalid electoral list ID.");

                if (string.IsNullOrWhiteSpace(liste.Lib))
                    throw new Exception(
                        "Electoral list name is missing.");

                if (liste.Candidats == null ||
                    liste.Candidats.Count == 0)
                    throw new Exception(
                        "No candidates found in list " +
                        liste.Lib);

                foreach (var candidat in liste.Candidats)
                {
                    if (candidat == null)
                        throw new Exception(
                            "Invalid candidate.");

                    if (candidat.Id <= 0)
                        throw new Exception(
                            "Invalid candidate ID.");

                    if (string.IsNullOrWhiteSpace(
                        candidat.FullNameAr))
                        throw new Exception(
                            "Candidate Arabic name is missing.");

                    if (candidat.Ordre <= 0)
                        throw new Exception(
                            "Invalid candidate order.");
                }
            }
        }

        private void ValidateUsers(ActivationPackage package)
        {
            if (package.Users == null ||
                package.Users.Count == 0)
                throw new Exception(
                    "No users found in activation file.");

            foreach (var user in package.Users)
            {
                if (user == null)
                    throw new Exception(
                        "Invalid user.");

                if (string.IsNullOrWhiteSpace(user.Id))
                    throw new Exception(
                        "User ID is missing.");

                if (string.IsNullOrWhiteSpace(user.UserName))
                    throw new Exception(
                        "UserName is missing.");

                if (string.IsNullOrWhiteSpace(user.PasswordHash) 
                    /*!user.PasswordHash.StartsWith("$2")*/)
                    throw new Exception(
                        "PasswordHash is missing or invalid.");

                if (string.IsNullOrWhiteSpace(user.Role))
                    throw new Exception(
                        "Role is missing.");

                if (string.IsNullOrWhiteSpace(user.CodeStructure))
                    throw new Exception(
                        "CodeStructure is missing.");
            }
        }

        // =========================================================
        // COMMUNES / CENTRES / BUREAUX
        // =========================================================

        private void ImportCommune(
       ActivationPackage package,
       CommuneDbContext db)
        {
            if (package.ActivationType == "APW")
            {
                foreach (var item in package.Communes)
                {
                    if (item == null)
                        throw new Exception("Invalid commune.");

                    if (string.IsNullOrWhiteSpace(item.CodeCommune))
                        throw new Exception("CodeCommune is missing.");

                    if (string.IsNullOrWhiteSpace(item.NameCommune))
                        throw new Exception("NameCommune is missing.");

                    if (string.IsNullOrWhiteSpace(item.CodeWilaya))
                        throw new Exception("CodeWilaya is missing.");

                    var existing = db.Cm.FirstOrDefault(
                        x => x.CC == item.CodeCommune);

                    if (existing != null)
                        throw new Exception(
                            "Commune already exists: " +
                            item.NameCommune);

                    var commune = new Cm
                    {
                        CC = item.CodeCommune,
                        CW = item.CodeWilaya,
                        NC = item.NameCommune
                    };

                    if (item.Centres != null)
                    {
                        foreach (var centreItem in item.Centres)
                        {
                            var centre = new Cn
                            {
                                Id = centreItem.Id,
                                C = centreItem.Code,
                                L = centreItem.Lib,
                                T = centreItem.Type,
                                NI = centreItem.NbInscrit
                            };

                            if (centreItem.Bureaux != null)
                            {
                                foreach (var bureauItem in centreItem.Bureaux)
                                {
                                    var bureau = new Bu
                                    {
                                        Id = bureauItem.Id,
                                        C = bureauItem.Code,
                                        T = bureauItem.Type,
                                        NI = bureauItem.NbInscrit
                                    };

                                    centre.Bux.Add(bureau);
                                }
                            }

                            commune.Cns.Add(centre);
                        }
                    }

                    db.Cm.Add(commune);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(
                    package.Commune.CodeCommune))
                    throw new Exception(
                        "CodeCommune is missing.");

                if (string.IsNullOrWhiteSpace(
                    package.Commune.NameCommune))
                    throw new Exception(
                        "NameCommune is missing.");

                if (string.IsNullOrWhiteSpace(
                    package.Commune.CodeWilaya))
                    throw new Exception(
                        "CodeWilaya is missing.");

                var existing = db.Cm.FirstOrDefault(
                    x => x.CC == package.Commune.CodeCommune);

                if (existing != null)
                    throw new Exception(
                        "Commune already exists: " +
                        package.Commune.NameCommune);

                var commune = new Cm
                {
                    CC = package.Commune.CodeCommune,
                    CW = package.Commune.CodeWilaya,
                    NC = package.Commune.NameCommune
                };

                if (package.Commune.Centres != null)
                {
                    foreach (var centreItem in package.Commune.Centres)
                    {
                        var centre = new Cn
                        {
                            Id = centreItem.Id,
                            C = centreItem.Code,
                            L = centreItem.Lib,
                            T = centreItem.Type,
                            NI = centreItem.NbInscrit
                        };

                        if (centreItem.Bureaux != null)
                        {
                            foreach (var bureauItem in centreItem.Bureaux)
                            {
                                centre.Bux.Add(new Bu
                                {
                                    Id = bureauItem.Id,
                                    C = bureauItem.Code,
                                    T = bureauItem.Type,
                                    NI = bureauItem.NbInscrit
                                });
                            }
                        }

                        commune.Cns.Add(centre);
                    }
                }

                db.Cm.Add(commune);
            }
        }


        // =========================================================
        // LISTES / CANDIDATS
        // =========================================================

        private void ImportListes(
     ActivationPackage package,
     CommuneDbContext db)
        {
            if (package.Listes == null)
                return;

            foreach (var item in package.Listes)
            {
                if (item == null)
                    throw new Exception("Invalid liste.");

                if (string.IsNullOrWhiteSpace(item.Lib))
                    throw new Exception(
                        "Liste libelle is missing.");

                var existingListe = db.LE.FirstOrDefault(
                    x => x.Id == item.Id);

                if (existingListe != null)
                    throw new Exception(
                        "Liste already exists: " +
                        item.Lib);

                var liste = new LE
                {
                    Id = item.Id,
                    L = item.Lib,
                    NI = item.NumIdentifiant
                };

                if (item.Candidats != null)
                {
                    foreach (var candidatItem in item.Candidats)
                    {
                        if (string.IsNullOrWhiteSpace(
                            candidatItem.FullNameAr))
                        {
                            throw new Exception(
                                "Candidat FullNameAr is missing in liste: "
                                + item.Lib);
                        }

                        if (candidatItem.Ordre <= 0)
                        {
                            throw new Exception(
                                "Candidat Ordre is invalid: "
                                + candidatItem.Ordre);
                        }

                        var existingCandidat = db.Cd.FirstOrDefault(
                            x => x.Id == candidatItem.Id);

                        if (existingCandidat != null)
                            throw new Exception(
                                "Candidat already exists: "
                                + candidatItem.FullNameAr);

                        var candidat = new Cd
                        {
                            Id = candidatItem.Id,
                            NA = candidatItem.FullNameAr,
                            NF = candidatItem.FullNameFr,
                            O = candidatItem.Ordre
                        };

                        liste.Cds.Add(candidat);
                    }
                }

                db.LE.Add(liste);
            }
        }


        // =========================================================
        // USERS
        // =========================================================

        private void ImportUsers(
     ActivationPackage package,
     CommuneDbContext db)
        {
            foreach (var item in package.Users)
            {
                if (string.IsNullOrWhiteSpace(item.UserName))
                    throw new Exception(
                        "UserName is missing.");

                if (string.IsNullOrWhiteSpace(item.PasswordHash))
                    throw new Exception(
                        "PasswordHash is missing.");

                if (string.IsNullOrWhiteSpace(item.Role))
                    throw new Exception(
                        "Role is missing.");

                var existing = db.U.FirstOrDefault(
                    x => x.Un == item.UserName);

                if (existing != null)
                    throw new Exception(
                        "User already exists: " +
                        item.UserName);

                var user = new U
                {
                    Id = item.Id,
                    Un = item.UserName,

                    PH = item.PasswordHash,

                    IA = true,

                    Rl = item.Role,

                    CS = item.CodeStructure
                };

                db.U.Add(user);
            }
        }
    }


    public class ActivationResult
    {
        public bool IsSuccess { get; private set; }

        public string Message { get; private set; }

        public string CodeCommune { get; private set; }

        public string CommuneName { get; private set; }


        public static ActivationResult Success(
            string code,
            string name)
        {
            return new ActivationResult
            {
                IsSuccess = true,
                CodeCommune = code,
                CommuneName = name,
                Message = "Activation successful."
            };
        }


        public static ActivationResult Fail(
            string message)
        {
            return new ActivationResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}