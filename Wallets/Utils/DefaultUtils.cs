namespace Wallets.Utils
{
    public static class DefaultUtils
    {
        public static TEnum? TryGet<TEnum>(this string value, out TEnum result) where TEnum : struct
        {
            var cond1 = Enum.TryParse(value, true, out result);
            var cond2 = Enum.IsDefined(typeof(TEnum), result);
            return cond1 && cond2 ? result : null;
        }
    }
}

