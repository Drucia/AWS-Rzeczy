namespace AWS_Rzeczy.Models
{
    public class Holder<T>
    {
        public T Value { get; }
        public string ErrorMsg { get; }
        public bool WasSuccessful { get;  }

        private Holder(T value)
        {
            WasSuccessful = true;
            Value = value;
        }

        private Holder(string errorMsg)
        {
            WasSuccessful = false;
            ErrorMsg = errorMsg;
        }

        public static Holder<T> Success(T value)
        {
            return new Holder<T>(value);
        }

        public static Holder<T> Fail(string errorMsg)
        {
            return new Holder<T>(errorMsg);
        }
    }
}
