using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SRGD.ViewModels;
using SRGD.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SRGD.Controllers
{
    public class UserController : Controller
    {
        private MasterDbContext _MasterDbContext;
        private IWebHostEnvironment _env;
 
        public UserController(MasterDbContext MasterDbContext, IWebHostEnvironment env)
        {
            _MasterDbContext = MasterDbContext;
            _env = env;
            
        }

        public ActionResult Index(Users user)
        {
           
            return View(user);
        }
     
        public ActionResult CreateExperiment(Users user,string Path)
        {
             var model = new Experiments();
            model.Username = user.Username;
            model.ExperimentFolderPath = Path;
            return View(model);
        }
        [HttpPost]
        public IActionResult CreateExperiment(Experiments exp)
        {
          
            int? expID = _MasterDbContext.experiments.Max(u => (int?)u.ExperimentID);
            if (expID != null)
            {
                expID ++;
            }
            else
            {
                expID = 1;
            }

            exp.ExperimentID =  expID ?? default(int); ;
            exp.ExperimentDate = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd"));
            exp.ExperimentTime = Convert.ToDateTime(System.DateTime.Now.ToString("HH:mm:ss"));
            exp.ExperimentFolderPath = (_env.WebRootPath + "/data/" + exp.Username + "/" + exp.ExperimentID + "-" + exp.Species).ToString();
   

            


            if (ModelState.IsValid)
             {
            
               var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
              _MasterDbContext.experiments.Add(exp);
                _MasterDbContext.SaveChanges();
                Directory.CreateDirectory(exp.ExperimentFolderPath);
                
                return RedirectToAction("Index", "Assembly",exp);
            }
             return View(exp);
        }
            
    }
}
