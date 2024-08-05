using System.ComponentModel.DataAnnotations;

namespace SIMS_APIs.Models {
    public class UpdateAccountRequest
    {
        public string? MemberCode { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PersonalPhone { get; set; }
        public string? ContactPhone1 { get; set; }
        public string? ContactPhone2 { get; set; }
        public string? PermanentAddress { get; set; }
        public string? TemporaryAddress { get; set; }
        public string? Major { get; set; }
        public string? ImagePath { get; set; }
        public string? Role { get; set; }

        public override string ToString()
        {
            return $"MemberCode: {MemberCode},RoleName: {Role}, Email: {Email}, Password: {Password}, Name: {Name}, Gender: {Gender}, DateOfBirth: {DateOfBirth}, PersonalPhone: {PersonalPhone}, ContactPhone1: {ContactPhone1}, ContactPhone2: {ContactPhone2}, PermanentAddress: {PermanentAddress}, TemporaryAddress: {TemporaryAddress}, Major: {Major}, ImagePath: {ImagePath}";
        }
    }
}
