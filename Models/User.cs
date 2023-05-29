using MaliehIran.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Models
{
    public class User : BaseEntity
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public long Type { get; set; }
        public int Status { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        [NotMapped]
        public string Role { get; set; }

        [NotMapped]
        public string ProfileImage { get; set; }
        [NotMapped]
        public string CurrentPassword { get; set; }
        [NotMapped]
        public List<Shop> Shops { get; set; }
    }
    public class UserSignDTO
    {
        public string Mobile { get; set; }
        public string Mail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string VerifyCode { get; set; }
        public string EncryptedMail { get; set; }
        public long Type { get; set; }
        public string UserName { get; set; }
        public long UserRole { get; set; }
    }
    public class UserAndUserSetting
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public long Type { get; set; }
        public int Status { get; set; }
        public long FormalId { get; set; }
        public string Bio { get; set; }
        public string BirthDate { get; set; }
        public int Gender { get; set; }
        public string UserTag { get; set; }
        public string Flag { get; set; }
        public bool IsPrivate { get; set; }

    }
    public class UserBaseData
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Banner { get; set; }
        public string Selfie { get; set; }
        public string UserName { get; set; }
    }

    public class ChangePasswordDto
    {
        public AuthTypes Type { get; set; }
        public string Phone { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class UserWithPaymentDto
    {
        public DateTime? SubscriptionExpireDate { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public long Type { get; set; }
        public int Status { get; set; }
        public long? FormalId { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int MyProperty { get; set; }
    }
}
