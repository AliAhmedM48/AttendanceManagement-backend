using Core.Enums;
using Core.ViewModels;

namespace Service;
public static class ServiceHelpers
{
    private static readonly Dictionary<string, string> GovernorateCodes = new()
    {
        ["01"] = "Cairo",
        ["02"] = "Alexandria",
        ["03"] = "Port Said",
        ["04"] = "Suez",
        ["11"] = "Damietta",
        ["12"] = "Dakahlia",
        ["13"] = "Sharqia",
        ["14"] = "Qalyubia",
        ["15"] = "Kafr El Sheikh",
        ["16"] = "Gharbia",
        ["17"] = "Monufia",
        ["18"] = "Beheira",
        ["19"] = "Ismailia",
        ["21"] = "Giza",
        ["22"] = "Beni Suef",
        ["23"] = "Fayoum",
        ["24"] = "Minya",
        ["25"] = "Asyut",
        ["26"] = "Sohag",
        ["27"] = "Qena",
        ["28"] = "Aswan",
        ["29"] = "Luxor",
        ["31"] = "Red Sea",
        ["32"] = "New Valley",
        ["33"] = "Matrouh",
        ["34"] = "North Sinai",
        ["35"] = "South Sinai",
        ["88"] = "Outside Egypt"
    };



    public static NationalIdInfo? ExtractNationalIdInfoFromNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId) || nationalId.Length != 14)
            return null;

        char centuryDigit = nationalId[0];
        int century = centuryDigit switch
        {
            '2' => 1900,
            '3' => 2000,
            _ => throw new ArgumentException("Invalid century digit in national ID.")
        };

        int year = century + int.Parse(nationalId.Substring(1, 2));
        int month = int.Parse(nationalId.Substring(3, 2));
        int day = int.Parse(nationalId.Substring(5, 2));

        DateTime birthDate;
        try
        {
            birthDate = new DateTime(year, month, day);
        }
        catch
        {
            throw new ArgumentException("Invalid birth date in national ID.");
        }

        string governorateCode = nationalId.Substring(7, 2);
        var governorate = GovernorateCodes.GetValueOrDefault(governorateCode, "Unknown");

        char genderDigit = nationalId[12];
        UserGender gender = (genderDigit % 2 == 0) ? UserGender.Female : UserGender.Male;

        return new NationalIdInfo
        {
            BirthDate = birthDate,
            Governorate = governorate,
            Gender = gender
        };
    }

}
