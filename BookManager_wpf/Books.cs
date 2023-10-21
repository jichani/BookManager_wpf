using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManager_wpf.Models
{
    public class Books
    {
        public int BookId { get; set; } // 책 ID
        public string Title { get; set; } // 책 제목
        public string Category { get; set; } // 카테고리 
        public string Author { get; set; } // 저자 
        public string Description { get; set; }  // 설명 
        public string Publisher { get; set; }  // 출판사 
        public string PublicationDate { get; set; }  // 출판일 (문자열)
        public string Quantity { get; set; }  // 수량 
        public DateTime RegisteredDate { get; set; }  // 등록일  

        // ToString 메서드 오버라이딩. 각 속성을 문자열로 변환하여 출력.
        public override string ToString()
        {
            return $"ID: {BookId}, Title: {Title}, Category: {Category}, Author: {Author}, Description: {Description}, Publisher: {Publisher}, PublicationDate: {PublicationDate}, Quantity: {Quantity}, RegisteredDate:{RegisteredDate}";
        }
    }
}
