using PeterO.Cbor;
using System.Globalization;

public static class CborManualParser
{
    public static CBORObject Parse(string input)
    {
        var dict = new Dictionary<object, object>();
        input = input.Trim();

        if (input.StartsWith("{") && input.EndsWith("}"))
            input = input.Substring(1, input.Length - 2).Trim();

        int i = 0;
        while (i < input.Length)
        {
            int colonIndex = FindCharOutsideBrackets(input, ':', i);
            if (colonIndex == -1) break;

            string keyPart = input.Substring(i, colonIndex - i).Trim();
            object key = ParseSingleValue(keyPart);

            i = colonIndex + 1;
            while (i < input.Length && (char.IsWhiteSpace(input[i]) || input[i] == ',')) i++;

            int valueEnd = FindValueEnd(input, i);
            string valuePart = input.Substring(i, valueEnd - i).Trim();
            dict[key] = ParseSingleValue(valuePart);

            i = valueEnd + 1;
        }

        var cbor = CBORObject.FromObject(dict);

        if (cbor.Count < 1)
        {
            throw new Exception("Not a CBOR!");
        }
        return cbor;
    }

    private static object ParseSingleValue(string val)
    {
        val = val.Trim();
        if (string.IsNullOrEmpty(val)) return null;

        // Nested Map {}
        if (val.StartsWith("{")) return Parse(val);

        // Nested Array []
        if (val.StartsWith("[")) return ParseArray(val);

        // Hex string h'...'
        if (val.StartsWith("h'")) return HexToByteArray(val.Trim('h', '\''));

        // Quoted string "..."
        if (val.StartsWith("\"")) return val.Trim('\"');

        // Booleans and Numbers
        if (val.ToLower() == "true") return true;
        if (val.ToLower() == "false") return false;
        if (long.TryParse(val, out long num)) return num;

        return val;
    }

    private static List<object> ParseArray(string input)
    {
        var list = new List<object>();
        string content = input.Substring(1, input.Length - 2).Trim();

        int i = 0;
        while (i < content.Length)
        {
            int end = FindValueEnd(content, i);
            string item = content.Substring(i, end - i).Trim();
            if (!string.IsNullOrEmpty(item))
                list.Add(ParseSingleValue(item));

            i = end + 1;
            while (i < content.Length && (char.IsWhiteSpace(content[i]) || content[i] == ',')) i++;
        }
        return list;
    }
    private static int FindValueEnd(string s, int start)
    {
        int braceLevel = 0;
        int bracketLevel = 0;
        bool inQuotes = false;

        for (int i = start; i < s.Length; i++)
        {
            char c = s[i];
            if (c == '\"') inQuotes = !inQuotes;
            if (inQuotes) continue;

            if (c == '{') braceLevel++;
            else if (c == '}') braceLevel--;
            else if (c == '[') bracketLevel++;
            else if (c == ']') bracketLevel--;
            else if (c == ',' && braceLevel == 0 && bracketLevel == 0) return i;
        }
        return s.Length;
    }

    private static int FindCharOutsideBrackets(string s, char target, int start)
    {
        int level = 0;
        for (int i = start; i < s.Length; i++)
        {
            if (s[i] == '{' || s[i] == '[') level++;
            else if (s[i] == '}' || s[i] == ']') level--;
            else if (s[i] == target && level == 0) return i;
        }
        return -1;
    }

    private static byte[] HexToByteArray(string hex)
    {
        if (hex.Length % 2 != 0) return Array.Empty<byte>();
        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < hex.Length; i += 2)
            bytes[i / 2] = byte.Parse(hex.Substring(i, 2), NumberStyles.HexNumber);
        return bytes;
    }
}