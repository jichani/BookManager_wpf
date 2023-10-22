using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManager_wpf.Models
{
    public class Checkouts
    {
        public int CheckoutId { get; set; } // 체크아웃 ID
        public int BookId { get; set; } // 책 ID
        public int MemberId { get; set; } // 회원 ID
        public string Name { get; set; } // 회원 이름
        public string Title { get; set; } // 책 제목
        public DateTime CheckoutDate { get; set; }  // 대출 날짜  
        public DateTime? ReturnDate { get; set; }  // 반납 날짜 (null 가능)

        public bool IsOverdue
        {
            get
            {
                // 만약 ReturnDate가 null(아직 반납되지 않았다)이고 CheckoutDate로부터 7일 이상 지났다면 true
                return !ReturnDate.HasValue && (DateTime.Now - CheckoutDate).TotalDays > 7;
            }
        }

        public bool IsReturned
        {
            get
            {
                return ReturnDate.HasValue;
            }
        }

        // ToString 메서드 오버라이딩. 각 속성을 문자열로 변환하여 출력.
        public override string ToString()
        {
            return $"Checkout ID: {CheckoutId}, Book ID: {BookId}, Member ID: {MemberId}, Checkout Date: {CheckoutDate}, Return Date: {(ReturnDate.HasValue ? ReturnDate.Value.ToString() : "Not returned yet")}, Overdue: {(IsOverdue ? "Yes" : "No")}";
        }
    }
}