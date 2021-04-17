using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using TestAppFromThirdLevelDomain.Models;

namespace TestAppFromThirdLevelDomain.Controllers
{
    public class EmpListsController : ApiController
    {
        private TestAppFromThirdLevelDomainContext db = new TestAppFromThirdLevelDomainContext();

        // GET: api/EmpLists
        public IQueryable<EmpList> GetEmpLists()
        {
            return db.EmpLists;
        }

        // GET: api/EmpLists/5
        [ResponseType(typeof(EmpList))]
        public IHttpActionResult GetEmpList(int id)
        {
            EmpList empList = db.EmpLists.Find(id);
            if (empList == null)
            {
                return NotFound();
            }

            return Ok(empList);
        }

        // PUT: api/EmpLists/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmpList(int id, EmpList empList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != empList.Id)
            {
                return BadRequest();
            }

            db.Entry(empList).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/EmpLists
        [ResponseType(typeof(EmpList))]
        public IHttpActionResult PostEmpList(EmpList empList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EmpLists.Add(empList);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = empList.Id }, empList);
        }


        [Route("api/upload/imagePost")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> PostUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 5; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        //var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        //var extension = ext.ToLower();
                        //if (!AllowedFileExtensions.Contains(extension))
                        //{

                        //    var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

                        //    dict.Add("error", message);
                        //    return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        //}
                        //else 
                        if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 1 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {



                            var filePath = HttpContext.Current.Server.MapPath("~/Images/" + postedFile.FileName);

                            postedFile.SaveAs(filePath);

                        }
                    }

                    var message1 = string.Format("Image Updated Successfully.");
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format("some Message");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }




        // DELETE: api/EmpLists/5
        [ResponseType(typeof(EmpList))]
        public IHttpActionResult DeleteEmpList(int id)
        {
            EmpList empList = db.EmpLists.Find(id);
            if (empList == null)
            {
                return NotFound();
            }

            db.EmpLists.Remove(empList);
            db.SaveChanges();

            return Ok(empList);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmpListExists(int id)
        {
            return db.EmpLists.Count(e => e.Id == id) > 0;
        }
    }
}