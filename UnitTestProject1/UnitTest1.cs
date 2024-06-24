using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnitTestProject1
{
    [TestClass]
    public class PasswordTests
    {
        [TestMethod]
        public void TestValidPassword()
        {
            string password = "Password1!";
            bool result = IsPasswordValid(password);
            Assert.IsTrue(result, "Valid password should return true.");
        }

        [TestMethod]
        public void TestPasswordTooShort()
        {
            string password = "Pass1!";
            bool result = IsPasswordValid(password);
            Assert.IsFalse(result, "Password too short should return false.");
        }

        [TestMethod]
        public void TestPasswordNoUppercase()
        {
            string password = "password1!";
            bool result = IsPasswordValid(password);
            Assert.IsFalse(result, "Password with no uppercase letter should return false.");
        }

        [TestMethod]
        public void TestPasswordNoDigit()
        {

            string password = "Password!";
            bool result = IsPasswordValid(password);
            Assert.IsFalse(result, "Password with no digit should return false.");
        }

        [TestMethod]
        public void TestPasswordNoSpecialCharacter()
        {
            string password = "Password1";
            bool result = IsPasswordValid(password);
            Assert.IsFalse(result, "Password with no special character should return false.");
        }

        private bool IsPasswordValid(string password)
        {
            // Проверка длины пароля
            if (password.Length < 6 && !Regex.IsMatch(password, "[A-Z]") && !Regex.IsMatch(password, "[0-9]") && !Regex.IsMatch(password, "[!@#$%^]"))
            {
                return false;
            }
            return true;
        }
    }
}