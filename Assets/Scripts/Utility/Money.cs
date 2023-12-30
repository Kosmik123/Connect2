
[System.Serializable]
public class Money
{
    public static string symbol = "$";
    public struct Prefix
    {
        public string fullName;
        public string shortName;

        public Prefix(string shortName, string fullName)
        {
            this.shortName = shortName;
            this.fullName = fullName;
        }
    }

    public static Prefix[] prefixes = {
        new Prefix("",""),
        new Prefix("k","kilo"), // k
        new Prefix("M","Mega"), // M
        new Prefix("G","Giga"), // B
        new Prefix("T","Tera"), // T
        new Prefix("P","Peta"), // Qu
        new Prefix("E","Exa"),  // qi
        new Prefix("Z","Zeta"), // Sx
        new Prefix("Y","Yotta"), // Sp
        new Prefix("R","Ronna"), // Oc
        new Prefix("Q","Quecca"),// No
        new Prefix("X","Xenna"), // Dc
        new Prefix("W","Weka"),  // UDc
        new Prefix("V","Vendeka"), // DDc
        new Prefix("D","Dodeka"),  // TDc
        new Prefix("L","Lasterra") // QDc
    };

    public const int HIGH_NUMBER = 1000;

    public ushort[] units = new ushort[16];

    public Money(long integer)
    {
        Add(integer);
    }

    public override string ToString()
    {
        int prefixIndex = 0;
        for (int j = 1; j < units.Length; j++)
        {
            if (units[j] > 0)
                prefixIndex = j;
        }

        float asFloating = units[prefixIndex] +
            (prefixIndex != 0 ? 1f * units[prefixIndex - 1] / HIGH_NUMBER : 0);

        string result = asFloating.ToString() + prefixes[prefixIndex];
        return result;
    }

    public void Add(long amount, int unit = 0)
    {
        int index = 0;
        long moneyToAdd = amount;

        while (moneyToAdd > HIGH_NUMBER)
        {
            long quotient = moneyToAdd / HIGH_NUMBER;
            ushort remainder = (ushort)(moneyToAdd - quotient * HIGH_NUMBER);
            units[index] += remainder;
            moneyToAdd = quotient;
        }

        Calculate();
    }

    public static bool operator >(Money a, Money b)
    {
        for (int i = a.units.Length; i >= 0; i++)
        {
            if (a.units[i] > b.units[i])
                return true;

            if (a.units[i] < b.units[i])
                return false;
        }
        return false;
    }

    public static bool operator <(Money a, Money b)
    {
        for (int i = a.units.Length; i >= 0; i++)
        {
            if (a.units[i] < b.units[i])
                return true;

            if (a.units[i] > b.units[i])
                return false;
        }
        return false;
    }

    public static bool operator >(Money a, long b)
    {
        Money mon = new Money(b);
        return a > mon;

    }

    public static bool operator <(Money a, long b)
    {
        Money mon = new Money(b);
        return a < mon;
    }

    private void Calculate()
    {
        for (int i = 0; i < units.Length; i++)
        {
            while (units[i] > HIGH_NUMBER)
            {
                units[i] -= HIGH_NUMBER;
                units[i + 1]++;
            }
        }
    }

    public static decimal DecimalPow(decimal baseNum, int power)
    {
        if (baseNum == 0)
            return 0;

        decimal result = 1;
        try
        {
            for (int i = 0; i < power; i++)
                result *= baseNum;
        }
        catch(System.OverflowException oe)
        {
            return decimal.MaxValue;
        }

        return result;
    }
}
