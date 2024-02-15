﻿using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Repository;
using BusinessLogic.Interfaces;
using DataAccess.CustomModel;
using DataAccess.DataContext;
using DataAccess.DataModels;
using Microsoft.EntityFrameworkCore;
using DataAccess.CustomModels;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Hallodocweb.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IAuth _Auth;
        private readonly ApplicationDbContext _context;
        private readonly IPatientService _patientService;
        private readonly INotyfService _notyf;
        public PatientController(ILogger<PatientController> logger, IAuth auth, ApplicationDbContext context, IPatientService patientService, INotyfService notyf)
        {
            _logger = logger;
            _Auth = auth;
            _context = context;
            _patientService = patientService;
             _notyf= notyf;
        }

        [HttpGet]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            bool emailExists = await _patientService.IsEmailExists(email);
            return Json(new { emailExists });
        }

        public IActionResult patientsubreq()
        {
            return View();
        }

        public IActionResult patientdashboard()
        {
            return View();
        }


        [HttpPost]
        public IActionResult patientreg(LoginVm loginVm)
        {
            if (_Auth.ValidateLogin(loginVm))
            {
                _notyf.Success("Success Notification");
                return RedirectToAction("patientdashboard", "patient");
            }
            else
            {
                _notyf.Error("Some Error Message");
                return View();
            }

        }

        public IActionResult patientreg()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult patientreq()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult patientreq(PatientInfoModel patientInfoModel)
        {
            if (ModelState.IsValid)
            {
                _patientService.AddPatientInfo(patientInfoModel);
                return RedirectToAction("patientsubreq");
            }
            else
            {
                return View(patientInfoModel);
            }
        }

        public IActionResult familyfriendreq()
        {
            return View();
        }

        [HttpPost]
        public IActionResult familyfriendreq(FamilyReqModel familyReqModel)
        {
            if (ModelState.IsValid)
            {
                _patientService.AddFamilyReq(familyReqModel);
                return RedirectToAction("patientsubreq", "Patient");
            }
            else
            {
                return View(familyReqModel);
            }

        }

        public IActionResult businessInforeq()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult businessInforeq(BusinessReqModel businessReqModel)
        {
            if (ModelState.IsValid)
            {
                _patientService.AddBusinessReq(businessReqModel);
                return RedirectToAction("patientsubreq", "Patient");
            }
            else
            {
                return View(businessReqModel);
            }
        }

        public IActionResult conciergereq()
        {
            return View();
        }

        [HttpPost]
        public IActionResult conciergereq(ConciergeReqModel conciergeReqModel)
        {
            if (ModelState.IsValid)
            {
                _patientService.AddConciergeReq(conciergeReqModel);
                return RedirectToAction("patientsubreq", "Patient");
            }
            else
            {
                return View(conciergeReqModel);
            }
        }

        public IActionResult patientfpassword()
        {
            return View();
        }

        public IActionResult patientsubinformation()
        {
            return View();
        }

        public IActionResult patientsomeoneelse ()
        {
            return View();
        }

        public IActionResult patientcreateacc()
        {
            return View();
        }
    }
}