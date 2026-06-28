using LR2Nexus.Utils;
using Microsoft.Data.Sqlite;

namespace LR2Nexus.Models;

public class DBPlayerRow(SqliteDataReader reader)
{
	public string? Id { get; set; } = DatabaseUtils.GetValue<string>(reader, "id");
	public string? Hash { get; set; } = DatabaseUtils.GetValue<string>(reader, "hash");
	public string? Name { get; set; } = DatabaseUtils.GetValue<string>(reader, "name");
	public int Playcount { get; set; } = DatabaseUtils.GetValue<int>(reader, "playcount");
	public int Clear { get; set; } = DatabaseUtils.GetValue<int>(reader, "clear");
	public int Fail { get; set; } = DatabaseUtils.GetValue<int>(reader, "fail");
	public int Perfect { get; set; } = DatabaseUtils.GetValue<int>(reader, "perfect");
	public int Great { get; set; } = DatabaseUtils.GetValue<int>(reader, "great");
	public int Good { get; set; } = DatabaseUtils.GetValue<int>(reader, "good");
	public int Bad { get; set; } = DatabaseUtils.GetValue<int>(reader, "bad");
	public int Poor { get; set; } = DatabaseUtils.GetValue<int>(reader, "poor");
	public int Playtime { get; set; } = DatabaseUtils.GetValue<int>(reader, "playtime");
	public int Combo { get; set; } = DatabaseUtils.GetValue<int>(reader, "combo");
	public int Maxcombo { get; set; } = DatabaseUtils.GetValue<int>(reader, "maxcombo");
	public int Grade7 { get; set; } = DatabaseUtils.GetValue<int>(reader, "grade_7");
	public int Grade5 { get; set; } = DatabaseUtils.GetValue<int>(reader, "grade_5");
	public int Grade14 { get; set; } = DatabaseUtils.GetValue<int>(reader, "grade_14");
	public int Grade10 { get; set; } = DatabaseUtils.GetValue<int>(reader, "grade_10");
	public int Grade9 { get; set; } = DatabaseUtils.GetValue<int>(reader, "grade_9");
	public int Trial { get; set; } = DatabaseUtils.GetValue<int>(reader, "trial");
	public int Option { get; set; } = DatabaseUtils.GetValue<int>(reader, "option");
	public int Systemversion { get; set; } = DatabaseUtils.GetValue<int>(reader, "systemversion");
	public int Gradeversion { get; set; } = DatabaseUtils.GetValue<int>(reader, "gradeversion");
	public int Trialversion { get; set; } = DatabaseUtils.GetValue<int>(reader, "trialversion");
	public string? Scorehash { get; set; } = DatabaseUtils.GetValue<string>(reader, "scorehash");

	public void SetId(string id)
	{
		Id = id;
		Name = id;
		UpdateScoreHash();
	}

	public void SetPassword(string password)
	{
		Hash = MD5Util.CalculateMD5(password).Body;
		UpdateScoreHash();
	}

	public void UpdateScoreHash()
	{
		Scorehash = CalculateScorehash().Body;
	}

	public MD5Hash CalculateScorehash()
	{
		return MD5Util.CalculateMD5(ToRawArray());
	}

	public object[] ToRawArray()
	{
		return
		[
			Bad,
            Clear,
            Combo,
            Fail,
            Good,
            Great,
            Maxcombo,
            Perfect,
            Playcount,
            Playtime,
            Poor,
            Hash ?? string.Empty,
            Grade9,
            Grade10,
            Grade14,
            Grade5,
            Grade7,
            Gradeversion,
            Trial,
            Trialversion,
            Systemversion,
            Option,
            "1"
        ];
	}
}