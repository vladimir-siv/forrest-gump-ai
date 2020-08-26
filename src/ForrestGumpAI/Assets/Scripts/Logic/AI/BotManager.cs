using System;
using System.Text;
using System.IO;

using GrandIntelligence;

public static class BotManager
{
	public const string Storage = @"D:\Desktop\projects\forrest-gump-ai\storage\";
	public static readonly string Replay = @"replay\bot.sav";
	public static readonly string Current = DateTime.Now.ToString("dd.MM.yyyy. HH-mm-ss");

	public static string SavePath(uint g) => Path.Combine(Current, $"bot-{g:0000}.sav");
	public static string UserPath(uint g, int k) => Path.Combine(Current, $"user-bot-{g:0000}-{k:0000}.sav");

	static BotManager()
	{
		Replay = Path.Combine(Storage, Replay);
		Current = Path.Combine(Storage, Current);
		if (!Directory.Exists(Current)) Directory.CreateDirectory(Current);
	}

	public static void Save(this BasicBrain brain, string file)
	{
		var sb = new StringBuilder();

		sb.AppendLine(brain.EvolutionValue.ToString());

		using (var it = new NeuralIterator())
		{
			for (var param = it.Begin(brain.NeuralNetwork); param != null; param = it.Next())
			{
				sb.Append($"{it.CurrentParam}:");
				var data = param.GetData();
				for (var i = 0; i < data.Length; ++i)
				{
					sb.Append($" {data[i]}");
				}
				sb.AppendLine();
			}
		}

		File.WriteAllText(file, sb.ToString());
	}
	public static void Load(this BasicBrain brain, string file)
	{
		var lines = File.ReadAllLines(file);

		var parameters = new float[lines.Length - 1][];

		for (var i = 1; i < lines.Length; ++i)
		{
			var split = lines[i].Split(':');

			var p = Convert.ToInt32(split[0]);

			var vals = split[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			parameters[p] = new float[vals.Length];

			for (var j = 0; j < vals.Length; ++j)
			{
				parameters[p][j] = Convert.ToSingle(vals[j]);
			}
		}

		using (var it = new NeuralIterator())
		{
			for (var param = it.Begin(brain.NeuralNetwork); param != null; param = it.Next())
			{
				param.Transfer(parameters[it.CurrentParam]);
			}
		}
	}
}
