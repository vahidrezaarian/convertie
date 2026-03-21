using PeterO.Cbor;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;

namespace Convertie.Utilities;

public static class Extensions
{
    public static string ToBase64UrlString(this byte[] data)
    {
        return Utils.ConvertByteArrayToBase64UrlString(data);
    }

    public static byte[] ToByteArrayFromBase64UrlString(this string data)
    {
        return Utils.ConvertBase64UrlStringToByteArray(data);
    }

    public static string ToBase64String(this byte[] data)
    {
        return Utils.ConvertByteArrayToBase64String(data);
    }

    public static byte[] ToByteArrayFromBase64String(this string data)
    {
        return Utils.ConvertBase64StringToByteArray(data);
    }

    public static string ToHexString(this byte[] data)
    {
        return Utils.ConvertByteArrayToHexString(data);
    }

    public static byte[] ToByteArrayFromHexString(this string data)
    {
        return Utils.ConvertHexStringToByteArray(data);
    }

    public static byte[] ComputeSha256(this byte[] data)
    {
        return Utils.ComputeSha256(data);
    }

    public static string ComputeSha256(this string data)
    {
        return Utils.ComputeSha256(data);
    }

    public static ECParameters ToElipticCurveParameters(this CBORObject key)
    {
        var point = new ECPoint
        {
            X = key[-2].GetByteString(),
            Y = key[-3].GetByteString()
        };

        return new ECParameters
        {
            Q = point,
            Curve = ECCurve.NamedCurves.nistP256
        };
    }

    public static Brush ToBrush(this string data)
    {
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(data));
    }

    public static byte[] GetBytesByEncoding(this string value, EncodingTypes encodingType)
    {
        if (encodingType == EncodingTypes.ASCII)
        {
            return Encoding.ASCII.GetBytes(value);
        }
        else if (encodingType == EncodingTypes.UTF32)
        {
            return Encoding.UTF32.GetBytes(value);
        }
        else if (encodingType == EncodingTypes.Unicode)
        {
            return Encoding.Unicode.GetBytes(value);
        }
        else if (encodingType == EncodingTypes.BigEndianUnicode)
        {
            return Encoding.BigEndianUnicode.GetBytes(value);
        }
        else if (encodingType == EncodingTypes.Latin1)
        {
            return Encoding.Latin1.GetBytes(value);
        }
        else
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }

    public static string GetStringByEncoding(this byte[] value, EncodingTypes encodingType)
    {
        if (encodingType == EncodingTypes.ASCII)
        {
            return Encoding.ASCII.GetString(value);
        }
        else if (encodingType == EncodingTypes.UTF32)
        {
            return Encoding.UTF32.GetString(value);
        }
        else if (encodingType == EncodingTypes.Unicode)
        {
            return Encoding.Unicode.GetString(value);
        }
        else if (encodingType == EncodingTypes.BigEndianUnicode)
        {
            return Encoding.BigEndianUnicode.GetString(value);
        }
        else if (encodingType == EncodingTypes.Latin1)
        {
            return Encoding.Latin1.GetString(value);
        }
        else
        {
            return Encoding.UTF8.GetString(value);
        }
    }

    public static string Convert(this string input, ConvertingTypes inputType, ConvertingTypes outputType, EncodingTypes encodingType)
    {
        switch (inputType)
        {
            case ConvertingTypes.Hex:
            case ConvertingTypes.Base64:
            case ConvertingTypes.Base64URL:
                var bytes = inputType == ConvertingTypes.Hex ? input.ToByteArrayFromHexString() : inputType == ConvertingTypes.Base64 ? input.ToByteArrayFromBase64String() : input.ToByteArrayFromBase64UrlString();

                if (outputType == ConvertingTypes.Hex)
                {
                    return bytes.ToHexString();
                }
                else if (outputType == ConvertingTypes.Base64)
                {
                    return bytes.ToBase64String();
                }
                else if (outputType == ConvertingTypes.Base64URL)
                {
                    return bytes.ToBase64UrlString();
                }
                else
                {
                    return bytes.GetStringByEncoding(encodingType);
                }
            case ConvertingTypes.CBOR:
                if (input.IsHexString())
                {
                    return CBORObject.DecodeFromBytes(input.ToByteArrayFromHexString()).ToString();
                }
                else if (input.IsBase64String())
                {
                    return CBORObject.DecodeFromBytes(input.ToByteArrayFromBase64String()).ToString();
                }
                else
                {
                    return CBORObject.DecodeFromBytes(input.ToByteArrayFromBase64UrlString()).ToString();
                }
            default:
                if (outputType == ConvertingTypes.Hex)
                {
                    return input.GetBytesByEncoding(encodingType).ToHexString();
                }
                else if (outputType == ConvertingTypes.Base64)
                {
                    return input.GetBytesByEncoding(encodingType).ToBase64String();
                }
                else if (outputType == ConvertingTypes.Base64URL)
                {
                    return input.GetBytesByEncoding(encodingType).ToBase64UrlString();
                }
                else if (outputType == ConvertingTypes.CBOR)
                {
                    return CborManualParser.Parse(input).EncodeToBytes().ToHexString();
                }
                return input;
        }
    }

    public static bool IsHexString(this string value)
    {
        try
        {
            value.ToByteArrayFromHexString();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsBase64String(this string value)
    {
        try
        {
            value.ToByteArrayFromBase64String();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsBase64UrlString(this string value)
    {
        try
        {
            value.ToByteArrayFromBase64UrlString();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsCborByteArray(this string value)
    {
        try
        {
            byte[] bytes;
            if (value.IsHexString())
            {
                bytes = value.ToByteArrayFromHexString();
            }
            else if (value.IsBase64String())
            {
                bytes = value.ToByteArrayFromBase64String();
            }
            else if (value.IsBase64UrlString())
            {
                bytes = value.ToByteArrayFromBase64UrlString();
            }
            else
            {
                return false;
            }

            CBORObject.DecodeFromBytes(bytes);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsCborByteArray(this byte[] value)
    {
        try
        {
            
            CBORObject.DecodeFromBytes(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool CanBeCborObject(this string value)
    {
        try
        {
            CborManualParser.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool ContainsCbor(this string value, out string detectedCborHexString)
    {
        detectedCborHexString = string.Empty;
        try
        {
            byte[] bytes;
            if (value.IsHexString())
            {
                bytes = value.ToByteArrayFromHexString();
            }
            else if (value.IsBase64String())
            {
                bytes = value.ToByteArrayFromBase64String();
            }
            else if (value.IsBase64UrlString())
            {
                bytes = value.ToByteArrayFromBase64UrlString();
            }
            else
            {
                return false;
            }

            using var ms = new MemoryStream(bytes);
            for (int i = 0; i < bytes.Length; i++)
            {
                ms.Position = i;
                try
                {
                    var result = CBORObject.Read(ms);
                    var cborBytes = result.EncodeToBytes();
                    if (result.Type == CBORType.Map && cborBytes.Length > 2)
                    {
                        detectedCborHexString = cborBytes.ToHexString();
                        return true;
                    }
                }
                catch { }
            }
        }
        catch { }
        return false;
    }
}

public static class Utils
{
    public static byte[] GetRandomBytes(int length)
    {
        byte[] bytes = new byte[length];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return bytes;
    }

    public static string ConvertByteArrayToHexString(byte[] byteArray)
    {
        return BitConverter.ToString(byteArray).Replace("-", "");
    }

    public static byte[] ConvertHexStringToByteArray(string hex)
    {
        int length = hex.Length;
        if (length % 2 != 0)
            throw new ArgumentException("Hex string must have an even number of characters.");

        byte[] byteArray = new byte[length / 2];
        for (int i = 0; i < length; i += 2)
        {
            byteArray[i / 2] = System.Convert.ToByte(hex.Substring(i, 2), 16);
        }

        return byteArray;
    }

    public static string ConvertByteArrayToBase64UrlString(byte[] data)
    {
        string base64 = System.Convert.ToBase64String(data);
        string base64Url = base64.Replace('+', '-')
                                 .Replace('/', '_')
                                 .TrimEnd('=');

        return base64Url;
    }

    public static byte[] ConvertBase64UrlStringToByteArray(string data)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentNullException(nameof(data));

        string base64 = data
            .Replace('-', '+')
            .Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return System.Convert.FromBase64String(base64);
    }

    public static string ConvertByteArrayToBase64String(byte[] data)
    {
        return System.Convert.ToBase64String(data, 0, data.Length);
    }

    public static byte[] ConvertBase64StringToByteArray(string data)
    {
        return System.Convert.FromBase64String(data);
    }

    public static string ConvertHexStringToBase64String(string data)
    {
        var bytes = ConvertHexStringToByteArray(data);
        return System.Convert.ToBase64String(bytes, 0, bytes.Length);
    }

    public static string ConvertBase64StringToHexString(string data)
    {
        return ConvertByteArrayToHexString(System.Convert.FromBase64String(data));
    }

    public static string ComputeSha256(string data)
    {
        var hash = SHA256.Create();
        var dataBytes = Encoding.UTF8.GetBytes(data);
        return hash.ComputeHash(dataBytes).ToHexString();
    }

    public static byte[] ComputeSha256(byte[] data)
    {
        var hash = SHA256.Create();
        return hash.ComputeHash(data);
    }

    public static byte[] Decrypt(byte[] cipherText, byte[] key, byte[] iv, PaddingMode padding = PaddingMode.None)
    {
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = padding;
            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
            using (MemoryStream ms = new MemoryStream(cipherText))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (MemoryStream output = new MemoryStream())
                    {
                        cs.CopyTo(output);
                        return output.ToArray();
                    }
                }
            }
        }
    }

    public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv, PaddingMode padding = PaddingMode.None)
    {
        byte[] encrypted;
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = padding;
            ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                }
                encrypted = ms.ToArray();
            }
        }
        return encrypted;
    }

    public static List<EncodingTypes> GetEncodingDecodingTypes()
    {
        return [EncodingTypes.UTF8, EncodingTypes.ASCII, EncodingTypes.UTF32, EncodingTypes.Unicode, EncodingTypes.BigEndianUnicode, EncodingTypes.Latin1 ];
    }

    public static List<ConvertingTypes> GetInputConvertingTypes(string input)
    {
        var list = new List<ConvertingTypes>();
        bool isCbor = false;
        bool isHex = false;
        bool isBase64 = false;
        bool isBase64Url = false;

        var task1 = Task.Run(() =>
        {
            isCbor = input.IsCborByteArray();
        });
        var task2 = Task.Run(() =>
        {
            isHex = input.IsHexString();
        });
        var task3 = Task.Run(() =>
        {
            isBase64 = input.IsBase64String();
        });
        var task4 = Task.Run(() =>
        {
            isBase64Url = input.IsBase64UrlString();
        });

        task1.Wait();
        task2.Wait();
        task3.Wait();
        task4.Wait();

        if (isCbor)
        {
            list.Add(ConvertingTypes.CBOR);
        }

        if (isHex)
        {
            list.Add(ConvertingTypes.Hex);
        }
        else if (isBase64)
        {
            list.Add(ConvertingTypes.Base64);
        }
        else if (isBase64Url)
        {
            list.Add(ConvertingTypes.Base64URL);
        }

        if (list.Count == 0)
        {
            list.Add(ConvertingTypes.Text);
        }

        return list;
    }

    public static List<ConvertingTypes> GetOutputConvertingTypes(string? input, ConvertingTypes inputType)
    {
        var list = new List<ConvertingTypes>();

        if (inputType == ConvertingTypes.Text)
        {
            if (input != null && input.CanBeCborObject())
            {
                list.Add(ConvertingTypes.CBOR);
            }
        }
        else
        {
            list.Add(ConvertingTypes.Text);
        }

        if (inputType != ConvertingTypes.CBOR)
        {
            if (inputType != ConvertingTypes.Hex)
            {
                list.Add(ConvertingTypes.Hex);
            }
            if (inputType != ConvertingTypes.Base64)
            {
                list.Add(ConvertingTypes.Base64);
            }
            if (inputType != ConvertingTypes.Base64URL)
            {
                list.Add(ConvertingTypes.Base64URL);
            }
        }

        return list;
    }
}
