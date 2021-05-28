using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MvcBoard.Models;
using Dapper;

namespace MvcBoard.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// 로그인
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                using (System.Data.IDbConnection db = new SqlConnection(Libs.Config.DBConnStrTest()))
                {
                    string sqlQuery = "SELECT * FROM userdb";
                    var user = db.Query(sqlQuery).FirstOrDefault(u => u.UserId.Equals(model.UserId) && 
                                                                      u.UserPassword.Equals(model.UserPassword));

                    if (user!=null)
                    {
                        // 로그인 성공시
                        return RedirectToAction("LoginSuccess", "Home");
                    }
                    // 로그인 실패시
                    ModelState.AddModelError(string.Empty, "사용자 ID 혹은 비밀번호가 옳바르지 않습니다");
                }
            }
            return View(model);
        }

        /// <summary>
        /// 회원 가입
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User model)
        {
            if(ModelState.IsValid)
            {
                using (System.Data.IDbConnection db = new SqlConnection(Libs.Config.DBConnStrTest()))
                {
                    string sqlQuery = "Insert Into userdb (UserId, UserPassword, UserName, RegisterDate) Values(@UserId, @UserPassword, @UserName, GETDATE())";
                    var rowsAffected = db.Execute(sqlQuery, new {UserId = model.UserId, UserPassword = model.UserPassword, UserName = model.UserName });
                }
                return RedirectToAction("Index", "Board");
            }
            return View(model);
        }
    }
}
