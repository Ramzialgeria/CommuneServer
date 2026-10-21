using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using CommuneServer.Data;
using CommuneServer.Data.Entities;
using CommuneServer.Models;

namespace CommuneServer.Controllers
{
    [Authorize]
    [RoutePrefix("api/client")]
    public class ClientController : ApiController
    {
        private readonly CommuneDbContext _db;

        public ClientController()
        {
            _db = new CommuneDbContext();
        }

        private string GetActivationType(ActivationInfo activation)
        {
            if (activation == null ||
                string.IsNullOrWhiteSpace(activation.CodeCommune))
            {
                return null;
            }

            if (activation.CodeCommune.Length == 2)
                return "APW";

            if (activation.CodeCommune.Length == 4)
                return "APC";

            return null;
        }

        private string CurrentCodeStructure
        {
            get
            {
                var identity = User.Identity as ClaimsIdentity;

                if (identity == null)
                    return null;

                var claim =
                    identity.FindFirst("code_structure");

                return claim != null
                    ? claim.Value
                    : null;
            }
        }

        private string CurrentUserId
        {
            get
            {
                var identity =
                    User.Identity as ClaimsIdentity;

                if (identity == null)
                    return null;

                var claim =
                    identity.FindFirst(ClaimTypes.NameIdentifier);

                string userId;

                if (claim != null)
                {
                    userId= claim.Value;

                    return userId;
                }

                return null;
            }
        }

        [HttpGet]
        [Route("activation")]
        public async Task<IHttpActionResult> GetActivation()
        {
            var activation = await _db.Activations.FirstOrDefaultAsync(x => x.IsActive);

            if (activation == null)
            {
                return Unauthorized();
            }

            string activationType = GetActivationType(activation);

            if (activationType == null)
            {
                return BadRequest("Type d'activation invalide.");
            }

            return Ok(new
            {
                ActivationType = activationType,
                CodeCommune = activation.CodeCommune
            });
        }

        [HttpGet]
        [Route("communes/available")]
        public async Task<IHttpActionResult> GetAvailableCommunes()
        {
            var activation = await _db.Activations.FirstOrDefaultAsync(x => x.IsActive);

            if (activation == null)
            {
                return Unauthorized();
            }

            string activationType = GetActivationType(activation);

            if (activationType != "APW")
            {
                return BadRequest("Cette installation n'est pas activée pour l'APW.");
            }

            // APW:
            // Activation.CodeCommune = CodeWilaya
            // Example: "01"
            string codeWilaya = activation.CodeCommune;

            if (string.IsNullOrWhiteSpace(codeWilaya) || codeWilaya.Length != 2)
            {
                return BadRequest("Code Wilaya d'activation invalide.");
            }

            var communes = await _db.Cm
                    .Where(x =>
                        x.CC != null &&
                        x.CC.StartsWith(codeWilaya))
                    .OrderBy(x => x.CC)
                    .Select(x => new
                    {
                        CodeCommune = x.CC,
                        NomCommune = x.NC
                    })
                    .ToListAsync();

            return Ok(communes);
        }


        [HttpGet]
        [Route("centres/available")]
        public async Task<IHttpActionResult> GetAvailableCentres(string codeCommune = null)
        {
            var activation = await _db.Activations.FirstOrDefaultAsync(x => x.IsActive);

            if (activation == null)
            {
                return Unauthorized();
            }

            string activationCode = activation.CodeCommune;

            if (string.IsNullOrWhiteSpace(activationCode))
            {
                return BadRequest("Code d'activation invalide.");
            }

            string activationType = GetActivationType(activation);

            if (activationType == null)
            {
                return BadRequest("Type d'activation invalide.");
            }

            // APC
            if (activationType == "APC")
            {
                // Activation.CodeCommune = commune
                codeCommune = activationCode;
            }
            else
            {
                // APW
                // Activation.CodeCommune = wilaya
                string codeWilaya = activationCode;

                if (string.IsNullOrWhiteSpace(codeCommune))
                {
                    return BadRequest("Le code commune est obligatoire pour l'APW.");
                }

                bool validCommune = await _db.Cm.AnyAsync(x =>
                        x.CC == codeCommune &&
                        x.CC.StartsWith(codeWilaya));

                if (!validCommune)
                {
                    return Unauthorized();
                }
            }
            try
            {

            
            //var centres = await _db.Cn
            //        .Where(x =>
            //            x.Cm.CC == codeCommune &&
            //            x.Bux.Any(b =>
            //                !_db.RB.Any(r =>
            //                    r.Bid == b.Id)))
            //        .OrderBy(x => x.C)
            //        .Select(x => new
            //        {
            //            Id = x.Id,
            //            Code = x.C,
            //            Lib = x.L
            //        })
            //        .ToListAsync();

                var centres = await _db.Cn
                    .Where(x =>
                        x.Cm.CC == codeCommune &&
                        x.Bux.Any(b =>
                            !_db.RB.Any(r =>
                                r.Bid == b.Id)))
                    .OrderBy(x => x.C)
                    .ToListAsync();

                var result = centres
                        .Select(x => new
                        {
                            Id = x.Id,
                            Code = x.C,
                            Type = x.T,
                            Lib = x.L,
                            NbInscrit = x.NI,
                            CodeCommune = x.Cm.CC
                        })
                        .ToList();
                
                return Ok(result);
               
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("bureaux/available")]
        public async Task<IHttpActionResult> GetAvailableBureaux(string codeCentre, string codeCommune = null)
        {
            if (string.IsNullOrWhiteSpace(codeCentre))
            {
                return BadRequest("Le code centre est obligatoire.");
            }

            var activation = await _db.Activations.FirstOrDefaultAsync(x => x.IsActive);

            if (activation == null)
            {
                return Unauthorized();
            }

            string activationCode = activation.CodeCommune;

            if (string.IsNullOrWhiteSpace(activationCode))
            {
                return BadRequest("Code d'activation invalide.");
            }

            string activationType = GetActivationType(activation);

            if (activationType == null)
            {
                return BadRequest("Type d'activation invalide.");
            }

            // APC
            if (activationType == "APC")
            {
                // Activation.CodeCommune = commune
                codeCommune = activationCode;
            }
            else
            {
                // APW
                // Activation.CodeCommune = wilaya
                string codeWilaya = activationCode;

                if (string.IsNullOrWhiteSpace(codeCommune))
                {
                    return BadRequest("Le code commune est obligatoire pour l'APW.");
                }

                bool validCommune = await _db.Cm.AnyAsync(x =>
                        x.CC == codeCommune &&
                        x.CC.StartsWith(codeWilaya));

                if (!validCommune)
                {
                    return Unauthorized();
                }
            }

            var bureaux = await _db.Bu
                    .Where(x =>
                        x.C != null &&
                        x.Cn.C == codeCentre &&
                        x.Cn.Cm.CC == codeCommune &&
                        !_db.RB.Any(r =>
                            r.Bid == x.Id))
                    .OrderBy(x => x.C)
                    .Select(x => new
                    {
                        Id = x.Id,
                        Code = x.C,
                        Type = x.T,
                        NbInscrit = x.NI
                    })
                    .ToListAsync();

            return Ok(bureaux);
        }

        [HttpGet]
        [Route("listes")]
        public async Task<IHttpActionResult> GetListes()
        {
            string codeStructure = CurrentCodeStructure;

            if (string.IsNullOrWhiteSpace(codeStructure))
                return BadRequest( "Code structure utilisateur introuvable.");

            var listes = await _db.LE
                .Where(x =>
                    !_db.RL.Any(r =>
                        r.Lid == x.Id))
                .OrderBy(x => x.NI)
                .ToListAsync();
            

            var result = listes
                    .Select(x => new
                    {
                        Id = x.Id,
                        Lib = x.L,
                        NumIdentifiant = x.NI
                    })
                    .ToList();

            return Ok(result);
        }


        [HttpGet]
        [Route("listes/{id:long}/candidats")]
        public async Task<IHttpActionResult> GetCandidats(long id)
        {
            if (id <= 0) 
                return BadRequest( "Identifiant de liste invalide.");

            var candidats = await _db.Cd
                .Where(x =>
                    x.LId == id &&
                    !_db.RC.Any(r =>
                        r.Cid == x.Id))
                .OrderBy(x => x.O)
                .ToListAsync();

            var result = candidats
                   .Select(x => new
                   {
                       Id = x.Id,
                       FullNameAr = x.NA,
                       FullNameFr = x.NF,
                       Ordre = x.O
                   })
                   .ToList();

            return Ok(result);
        }

        //[Authorize(Roles = "Saisie")]
        //[HttpPost]
        //[Route("results")]
        //public IHttpActionResult SaveResults(ResultRequest request)
        //{
        //    // ...

        //    return Ok();
        //}

        [Authorize(Roles = "Magistra")]
        [HttpPost]
        [Route("results/{id}/validate")]
        public IHttpActionResult ValidateResult(int id)
        {
            // ...

            return Ok();
        }
    

}
}