using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DataAccess.DataContext;
using BusinessLogic.Interface;
using DataAccess.Customemodel;
using DataAccess.DataModels;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IPatientService _petientService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db,IPatientService patientService)
        {
            _logger = logger;
            _db = db;
            _petientService = patientService;
        }

        public IActionResult Index()
        {
            var data = _petientService.patientinfo();
            return View(data);
        }

        public IActionResult PatientForm()
        {
            return PartialView("_PatientForm");
        }

        [HttpPost]
        public IActionResult SubmitPatientForm(Patientform patientform,string myRadio)
        {
            _petientService.SubmitPatientform(patientform);
            return RedirectToAction("Index");
        }

        public IActionResult DeletePatient(int patientid)
        {
            _petientService.DeletePatient(patientid);
            return RedirectToAction("Index");
        }

        public IActionResult UpdatePatient(int patientid)
        {
            var data = _petientService.GetPatient(patientid);
            return PartialView("_EditPatientForm",data);
        }

        [HttpPost]
        public IActionResult UpdatePatientSubmit(Patientform patientform,int patientid)
        {
            _petientService.UpdatePatient(patientform,patientid);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}