using System;
using System.Text;
using System.IO;
using System.Xml;

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

	public static void Save(this NeatBrain brain, string file)
	{
		var doc = new XmlDocument();
		{
			doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

			var root = doc.AppendChild(doc.CreateElement("neat_brain"));
			{
				var spec = root.AppendChild(doc.CreateElement("spec"));
				{
					var inputs = spec.AppendChild(doc.CreateElement("inputs"));
					inputs.AppendChild(doc.CreateTextNode($"{brain.Inputs}"));
					
					var hidden = spec.AppendChild(doc.CreateElement("hidden"));
					hidden.AppendChild(doc.CreateTextNode($"{brain.Hidden}"));
					
					var outputs = spec.AppendChild(doc.CreateElement("outputs"));
					outputs.AppendChild(doc.CreateTextNode($"{brain.Outputs}"));
					
					var hidden_activation = spec.AppendChild(doc.CreateElement("hidden_activation"));
					hidden_activation.AppendChild(doc.CreateTextNode($"{brain.HiddenActivation}"));
					
					var activation = spec.AppendChild(doc.CreateElement("activation"));
					activation.AppendChild(doc.CreateTextNode($"{brain.Activation}"));
				}

				var genes = root.AppendChild(doc.CreateElement("genes"));
				{
					var nodes = genes.AppendChild(doc.CreateElement("nodes"));
					{
						for (var i = 0u; i < brain.NodeGenes; ++i)
						{
							var nodegene = brain.NodeGene(i);

							var node = nodes.AppendChild(doc.CreateElement("node"));
							{
								var type = node.AppendChild(doc.CreateElement("type"));
								type.AppendChild(doc.CreateTextNode($"{nodegene.Type}"));

								var bias = node.AppendChild(doc.CreateElement("bias"));
								bias.AppendChild(doc.CreateTextNode($"{nodegene.Bias}"));
							}
						}
					}

					var connections = genes.AppendChild(doc.CreateElement("connections"));
					{
						for (var i = 0u; i < brain.ConnectionGenes; ++i)
						{
							var connectiongene = brain.ConnectionGene(i);

							var connection = connections.AppendChild(doc.CreateElement("connection"));
							{
								var inn = connection.AppendChild(doc.CreateElement("in"));
								inn.AppendChild(doc.CreateTextNode($"{connectiongene.In}"));

								var outn = connection.AppendChild(doc.CreateElement("out"));
								outn.AppendChild(doc.CreateTextNode($"{connectiongene.Out}"));

								var weight = connection.AppendChild(doc.CreateElement("weight"));
								weight.AppendChild(doc.CreateTextNode($"{connectiongene.Weight}"));

								var enabled = connection.AppendChild(doc.CreateElement("enabled"));
								enabled.AppendChild(doc.CreateTextNode($"{connectiongene.Enabled}"));
							}
						}
					}
				}
			}
		}

		doc.Save(file);
	}
	public static NeatBrain LoadNeatBrain(string file)
	{
		NeatBrain brain = null;

		var doc = new XmlDocument();
		doc.LoadXml(File.ReadAllText(file));

		var root = doc.FirstChild;

		if (root is XmlDeclaration) root = root.NextSibling;

		if (root.Name != "neat_brain") throw new FormatException("Invalid root element!");
		{
			var spec = root.FirstChild;
			if (spec.Name != "spec") throw new FormatException("Invalid spec!");
			{
				var inputs = spec.FirstChild;
				if (inputs.Name != "inputs") throw new FormatException("Invalid inputs!");
				var inputs_value = Convert.ToUInt32(inputs.FirstChild.Value);
				
				var hidden = inputs.NextSibling;
				if (hidden.Name != "hidden") throw new FormatException("Invalid hidden!");
				var hidden_value = Convert.ToUInt32(hidden.FirstChild.Value);
				
				var outputs = hidden.NextSibling;
				if (outputs.Name != "outputs") throw new FormatException("Invalid outputs!");
				var outputs_value = Convert.ToUInt32(outputs.FirstChild.Value);
				
				var hidden_activation = outputs.NextSibling;
				if (hidden_activation.Name != "hidden_activation") throw new FormatException("Invalid hidden_activation!");
				var hidden_activation_value = (ActivationFunction)Enum.Parse(typeof(ActivationFunction), hidden_activation.FirstChild.Value);

				var activation = hidden_activation.NextSibling;
				if (activation.Name != "activation") throw new FormatException("Invalid activation!");
				var activation_value = (ActivationFunction)Enum.Parse(typeof(ActivationFunction), activation.FirstChild.Value);

				if (hidden_value != 0u) brain = new NeatBrain(inputs_value, hidden_value, outputs_value, hidden_activation_value, activation_value);
				else brain = new NeatBrain(inputs_value, outputs_value, hidden_activation_value, activation_value);
			}

			if (brain == null) throw new InvalidOperationException("Something went wrong while loading.");

			var genes = spec.NextSibling;
			if (genes.Name != "genes") throw new FormatException("Invalid genes!");
			{
				var nodes = genes.FirstChild;
				if (nodes.Name != "nodes") throw new FormatException("Invalid nodes!");
				{
					for (var node = nodes.FirstChild; node != null; node = node.NextSibling)
					{
						if (node.Name != "node") throw new FormatException("Invalid node!");
						{
							var type = node.FirstChild;
							if (type.Name != "type") throw new FormatException("Invalid type!");
							var type_value = (NodeType)Enum.Parse(typeof(NodeType), type.FirstChild.Value);

							var bias = type.NextSibling;
							if (bias.Name != "bias") throw new FormatException("Invalid bias!");
							var bias_value = Convert.ToSingle(bias.FirstChild.Value);

							brain.AddNode(type_value, bias_value);
						}
					}
				}

				var connections = nodes.NextSibling;
				if (connections.Name != "connections") throw new FormatException("Invalid connections!");
				{
					for (var connection = connections.FirstChild; connection != null; connection = connection.NextSibling)
					{
						if (connection.Name != "connection") throw new FormatException("Invalid connection!");
						{
							var inn = connection.FirstChild;
							if (inn.Name != "in") throw new FormatException("Invalid in!");
							var inn_value = Convert.ToUInt32(inn.FirstChild.Value);

							var outn = inn.NextSibling;
							if (outn.Name != "out") throw new FormatException("Invalid out!");
							var outn_value = Convert.ToUInt32(outn.FirstChild.Value);

							var weight = outn.NextSibling;
							if (weight.Name != "weight") throw new FormatException("Invalid weight!");
							var weight_value = Convert.ToSingle(weight.FirstChild.Value);

							var enabled = weight.NextSibling;
							if (enabled.Name != "enabled") throw new FormatException("Invalid enabled!");
							var enabled_value = Convert.ToBoolean(enabled.FirstChild.Value);

							brain.AddConnection(inn_value, outn_value, weight_value, enabled_value);
						}
					}
				}
			}
		}

		return brain;
	}
}
