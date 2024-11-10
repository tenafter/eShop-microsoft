namespace eShop.Identity.API.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    /// <summary>
    /// 애플리케이션 사용자 정보를 나타내는 클래스입니다.
    /// IdentityUser를 상속받아 추가적인 사용자 프로필 데이터를 저장합니다.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 신용카드 번호
        /// </summary>
        [Required]
        public string CardNumber { get; set; }

        /// <summary>
        /// 신용카드 보안 번호
        /// </summary>
        [Required] 
        public string SecurityNumber { get; set; }

        /// <summary>
        /// 신용카드 만료일 (MM/YY 형식)
        /// </summary>
        [Required]
        [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match a valid MM/YY value")]
        public string Expiration { get; set; }

        /// <summary>
        /// 카드 소유자 이름
        /// </summary>
        [Required]
        public string CardHolderName { get; set; }

        /// <summary>
        /// 카드 유형
        /// </summary>
        public int CardType { get; set; }

        /// <summary>
        /// 거리 주소
        /// </summary>
        [Required]
        public string Street { get; set; }

        /// <summary>
        /// 도시
        /// </summary>
        [Required]
        public string City { get; set; }

        /// <summary>
        /// 주/도
        /// </summary>
        [Required]
        public string State { get; set; }

        /// <summary>
        /// 국가
        /// </summary>
        [Required]
        public string Country { get; set; }

        /// <summary>
        /// 우편번호
        /// </summary>
        [Required]
        public string ZipCode { get; set; }

        /// <summary>
        /// 이름
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 성
        /// </summary>
        [Required]
        public string LastName { get; set; }
    }
}
