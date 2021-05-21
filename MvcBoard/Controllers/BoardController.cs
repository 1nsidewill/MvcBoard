using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Dapper;
using MvcBoard.Models;
using PagedList;

namespace MvcBoard.Controllers
{
    public class BoardController : Controller
    {
        // GET: Board
        public ActionResult Index(string CurrentSchTxt, string SchTxt, int? page)
        {
            List<Board> boardList = new List<Board>();
            if (SchTxt != null)
            {
                page = 1;
            }
            else
            {
                // 신규 검색어가 아닌 기존 검색어가 넘어올 경우 기존 검색어를 SchTxt에 넣어준다.
                // CurrentFilter 새 검색어가 입력되기 전까지 검색어는 이 녀석이 가지고 있다.
                // 그리고 가지고 있는 값을 매번 SchTxt에 넘겨준다.
                SchTxt = CurrentSchTxt;
            }
            if (SchTxt == null)
            {
                SchTxt = "";
            }
            ViewBag.CurrentFilter = SchTxt;
            using (IDbConnection db = new SqlConnection(Libs.Config.DBConnStrTest()))
            {
                var boardData = db.Query<Board>("Select * From mvcboard " +
                                                "WHERE (@SchTxt = '' OR (@SchTxt <> '' AND board_subject = @SchTxt)) " +
                                                "ORDER BY board_postNo DESC", new { @SchTxt = SchTxt });
                //boardData = boardData.Contains
                boardList = boardData.ToList();

            }

            // 현재 페이지 정보가 없다면 1페이지로 간주하고 아니면 페이지 정보를 넘긴다.
            int pageNo = page ?? 1;

            // pageSize : 한 페이지에 불러올 컨텐츠의 수
            int pageSize = 5;

            return View(boardList.ToPagedList(pageNo, pageSize));
        }


        // GET: Board/Details/5
        public ActionResult Details(int? id)
        {
            Board _board = new Board();
            using (IDbConnection db = new SqlConnection(Libs.Config.DBConnStrTest()))
            {
                string test = @"
                    DECLARE @board_count INT 
                    SELECT @board_count = (board_readCount + 1) FROM mvcboard WHERE board_postNo = @id

                    UPDATE mvcboard SET board_readCount = @board_count WHERE board_postNo = @id

                    SELECT
                        *
                    FROM
                        mvcboard
                    WHERE
                        board_postNo = @id


                ";

                _board = db.Query<Board>(test, new {@id = id }).SingleOrDefault();


            }

            return View(_board);
        }

        // GET: Board/Create
        // GET: Board/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Board/Create
        [HttpPost]
        public ActionResult Create(Board _board)
        {

            using (IDbConnection db = new SqlConnection(Libs.Config.DBConnStrTest()))
            {
                string sqlQuery = "Insert Into mvcboard (board_name, board_subject, board_content, board_writeTime) Values(@board_name, @board_subject, @board_content, GETDATE())";
                var rowsAffected = db.Execute(sqlQuery, new {board_name = _board.board_name, board_subject=_board.board_subject, board_content = _board.board_content});
            }

            return RedirectToAction("Index");
        }

        // GET: Board/Edit/5  
        public ActionResult Edit(int id)
        {
            Board _board = new Board();
            using (IDbConnection db = new SqlConnection(Libs.Config.DBConnStrTest()))
            {
                _board = db.Query<Board>("Select * From mvcboard " +
                                       "WHERE board_postNo =" + id, new { id }).SingleOrDefault();
            }
            return View(_board);
        }

        // POST: Board/Edit/5  
        [HttpPost]
        public ActionResult Edit(Board _board)
        {
            using (IDbConnection db = new SqlConnection(Libs.Config.DBConnStrTest()))
            {

                string sqlQuery = "update mvcboard set board_name='" + _board.board_name + "',board_subject='" + _board.board_subject + "',board_content='" + _board.board_content + "' where board_postNo=" + _board.board_postNo;

                int rowsAffected = db.Execute(sqlQuery);
            }

            return RedirectToAction("Index");
        }

        // GET: Board/Delete/5  
        public ActionResult Delete(int id)
        {
            Board _board = new Board();
            using (IDbConnection db = new SqlConnection(Libs.Config.DBConnStrTest()))
            {
                _board = db.Query<Board>("Select * From mvcboard " +
                                       "WHERE board_postNo =" + id, new { id }).SingleOrDefault();
            }
            return View(_board);
        }

        // POST: Board/Delete/5  
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            using (IDbConnection db = new SqlConnection(Libs.Config.DBConnStrTest()))
            {
                string sqlQuery = "Delete From mvcboard WHERE board_postNo = " + id;

                int rowsAffected = db.Execute(sqlQuery);


            }
            return RedirectToAction("Index");
        }
    }
}