namespace SmartHouse.Core.Messages
{
    public class ErrorMessages
    {
        public static string NotFound(string type, string attribute) => String.Format("The {0} with the given {1} doesn't exist.", type, attribute);
        public static string NotFoundUserId() => "The User with the given Id doesn't exist.";
        public static string AlreadyExists(string type, string attribute) => String.Format("The {0} with the given {1} already exists.", type, attribute);
        public static string EmailNotInCorrectFormat() => "The email is not in the correct format (example@mail.com).";
        public static string OldPasswordIsIncorrect() => "The old password is incorect. Try again.";

        public static string EmailNotConfirmed() => "The email is not confirmed. Please check your mail box.";
        public static string PasswordsDoNotMatch() => "Password and repeat password do not match";
        public static string OldAndNewPasswordAreTheSame() => "Old and new password cannot be the same.";
        public static string ExistingEmailOrUsername() => "Account with this email or username already exists";
        public static string AlreadyProcessedProperty() => "The property is already processed.";

        public static string ExitingAccountNotVerified() => "Your account is registered, but not verified, check your mailbox";

        public static string ExistingLicencePlate() => "Your licence plate is already valid";

        public static string ExistingSprinklerDay() => "Your day is already valid day";

        public static string InvalidDay() => "Your day is not a valid day";

        public static string NonExistingLicencePlate() => "Your licence plate is already not valid";

        public static string NonExistingSprinklerDay() => "Your sprinkler day is already not valid";

        public static string InvalidTime() => "Your time is not in valid format";

    }
}
