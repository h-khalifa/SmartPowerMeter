using PowerReadingsDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Linq;

namespace PowerReadingsDemo.Controllers
{
    public class PowerMeterController : Controller
    {
        // GET: PowerMeter
        public ActionResult Index()
        {
            PowerMeterDemoEntities db = new PowerMeterDemoEntities();
            var results = db.Measures.ToList();
            int x = 43;
            int y = results.Count - x;
            return View(results);
        }

        //public ActionResult GetReadings()
        //{
        //    PowerMeterDemoEntities db = new PowerMeterDemoEntities();
        //    var results = db.Measures.Select(m => new
        //    {
        //        MeasureName = m.MeasureName,
        //        MeasureUnit = m.MeasureUnit,
        //        Readings = m.ReadingLKPs.Select(r => new
        //        {
        //            ReadingName = r.Symbol,
        //            ReadingId = r.Id,
        //            Values = r.Readings.Select(v => new
        //            {
        //                Value = v.ReadingVal,
        //                ValueTime = v.ReadingTime
        //            }),
        //        })
        //    }).ToList();
        //    return Json(results, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetReadingLkp(int RID)
        {
            PowerMeterDemoEntities db = new PowerMeterDemoEntities();
            var readingLkp = db.ReadingLKPs.Where(r => r.Id == RID).AsEnumerable().Select(r => new
            {
                Name = r.Symbol,
                Unit = r.Measure.MeasureUnit,
                Readings = r.Readings.Where(rv => rv.ReadingTime.Value.Minute == 0).Select(rv => new {
                    Value = rv.ReadingVal,
                    //ValueTime = rv.ReadingTime.Value.ToString("dd/MM/yyyy hh:mm"),
                    ValueTime = rv.ReadingTime.Value.ToString("hh:mm tt"),
                }),
            }).FirstOrDefault();
            return Json(readingLkp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMeasureReadings(int MID)
        {
            PowerMeterDemoEntities db = new PowerMeterDemoEntities();
            var result = db.ReadingLKPs.Where(r => r.MeaasureID == MID).AsEnumerable().Select(r => new
            {
                Name = r.Symbol,
                Unit = r.Measure.MeasureUnit,
                Readings = r.Readings.Where(rv => rv.ReadingTime.Value.Minute == 0).Select(rv => new {
                    Value = rv.ReadingVal,
                    //ValueTime = rv.ReadingTime.Value.ToString("dd/MM/yyyy hh:mm"),
                    ValueTime = rv.ReadingTime.Value.ToString("hh:mm tt"),
                }).ToArray()
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Config()
        {
            PowerMeterDemoEntities db = new PowerMeterDemoEntities();
            var model = db.ReadingLKPs.Select(lkp => new Models.ViewModels.PowerMeterController.ConfigAction.ReadingLKPViewModel()
            {
                Id = lkp.Id,
                Symbol = lkp.Symbol + " - " + lkp.Measure.MeasureUnit
            }).ToList();
            return View(model);
        }

        public ActionResult GetReadingThreshold(int RID)
        {
            PowerMeterDemoEntities db = new PowerMeterDemoEntities();
            var reading = db.ReadingLKPs.Find(RID);
            var result = reading.Threshold;
            return Json(result==null? -1 : result, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult SaveReadingThreshold(int RID, Nullable< decimal> thrshld)
        {
            PowerMeterDemoEntities db = new PowerMeterDemoEntities();
            var reading = db.ReadingLKPs.Find(RID);
            reading.Threshold = thrshld;
            db.Entry<ReadingLKP>(reading).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        public ActionResult Survey()
        {
            return View();
        }

        //[System.Web.Mvc.HttpPost]
        public ActionResult GetReadingsSurvey(string Ids)
        {
            //Ids = ["10","14","18"]
            Ids = Ids.TrimStart('[').TrimEnd(']');
            string[] SIds = Ids.Split(',');

            List<int> ids = new List<int>();
            foreach(string sId in SIds)
            {
                ids.Add(int.Parse(sId.Trim('\"')));
            }

            PowerMeterDemoEntities db = new PowerMeterDemoEntities();
            var result = db.ReadingLKPs.Where(lkp => ids.Contains(lkp.Id)).AsEnumerable()
                .Select(lkp => new
                {
                    Name = lkp.Symbol,
                    Unit = lkp.Measure.MeasureUnit,
                    Readings = lkp.Readings.Where(rv => rv.ReadingTime.Value.Minute == 0).Select(rv => new
                    {
                        Value = rv.ReadingVal,
                        ValueTime = rv.ReadingTime.Value.ToString("hh:mm tt"),
                    }).ToArray()
                }).ToArray();

           
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}