using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TaskOrganizer.database;
using TaskOrganizer.Models;

namespace TaskOrganizer.Report
{
    public class TaskReport
    {
        #region declaration
        Document _document;
        Font _FontStyle;
        PdfPTable _pdftable = new PdfPTable(4);
        PdfPCell _pdfPCell;
        MemoryStream _memoryStream = new MemoryStream();


        #endregion
        public byte[] PrepareReport(Task task)
        {
            _document = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
            _pdftable.WidthPercentage = 100;
            _FontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
            PdfWriter.GetInstance(_document, _memoryStream);
            _FontStyle = FontFactory.GetFont("Tahoma", 10f, 0);
            //Chunk heading = new Chunk("Task Report", _FontStyle);
            //Chunk date = new Chunk(DateTime.Now.ToString());


            Paragraph para = new Paragraph();
            _document.Open();
            Chunk glue = new Chunk(new VerticalPositionMark());
            Phrase ph1 = new Phrase();
            ph1.Add(new Chunk(Environment.NewLine));
            string heading = "Task Report";
            string date = DateTime.Now.ToString();
            Paragraph main = new Paragraph();
            ph1.Add(new Chunk(heading)); // Here I add projectname as a chunk into Phrase.    
            ph1.Add(glue); // Here I add special chunk to the same phrase.    
            ph1.Add(new Chunk(date)); // Here I add date as a chunk into same phrase.    
            main.Add(ph1);
            para.Add(main);
            _document.Add(para);
            _document.Add(new Chunk(Environment.NewLine));
          
            _pdftable.SetWidths(new float[] { 145f,35f,30f,20f});
            this.RoportHeader(task);
            this.ReportBody(task);
           
            _pdftable.HeaderRows = 2;
           
            _document.Close();
            return _memoryStream.ToArray();
        }

        private void ReportBody(Task task)
        {
            List<Task> Tasks = new List<Task>();
            
            using (ETOEntities eto = new ETOEntities())
            {
                if (task.Assignee == "All")
                {
                    var result = eto.sp_GetFilteredData("-1", task.Stage, task.Priority, task.FromDate, task.ToDate).ToList();
                    foreach (var item in result)
                    {
                        Task tsk = new Task();
                        tsk.Assignee = item.Assignee;
                        tsk.Stage = item.Stage;
                        tsk.DueDate = (DateTime)item.DueDate;
                        tsk.TaskName = item.TaskName;
                        Tasks.Add(tsk);
                    }
                }
                else
                {
                    var result = eto.sp_GetFilteredData(task.Assignee, task.Stage, task.Priority, task.FromDate, task.ToDate).ToList();
                    foreach (var item in result)
                    {
                        Task tsk = new Task();
                        tsk.Assignee = item.Assignee;
                        tsk.Stage = item.Stage;
                        tsk.DueDate = (DateTime)item.DueDate;
                        tsk.TaskName = item.TaskName;
                        Tasks.Add(tsk);
                    }
                }
                _FontStyle = FontFactory.GetFont("Tahoma", 8f, 0);
                foreach (var item in Tasks)
                {
                    _pdfPCell = new PdfPCell(new Phrase(item.TaskName, _FontStyle));
                    _pdftable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(item.Assignee, _FontStyle));
                    _pdftable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(item.Stage, _FontStyle));
                    _pdftable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(item.DueDate.ToString("dd/MM/yyyy"), _FontStyle));
                    _pdftable.AddCell(_pdfPCell);

                    _pdftable.CompleteRow();
                }

            }
          
            _document.Add(_pdftable);
        }

        private void RoportHeader(Task task)
        {
            _FontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Task Name", _FontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.LightGray;
            _pdftable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Assignee", _FontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.LightGray;
            _pdftable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Stage", _FontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.LightGray;
            _pdftable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Due Date", _FontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.LightGray;
            _pdftable.AddCell(_pdfPCell);
        }
    }
}