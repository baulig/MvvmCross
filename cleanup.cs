using System;
using System.IO;
using System.Text.RegularExpressions;

class X {
	static void Main (string[] args)
	{
		foreach (var file in args)
			Cleanup (file);
	}

	static void Cleanup (string file)
	{
		var tmp = Path.GetTempFileName ();

		using (var input = new StreamReader (file)) {
			using (var output = new StreamWriter (tmp)) {
				Cleanup (input, output);
			}
		}

		File.Delete (file);
		File.Move (tmp, file);
	}

	static string nonWindowsRe = @"<TargetFrameworkProfile Condition=.*!=";
	static string windowsRe = "^(\\s*)<TargetFrameworkProfile Condition=\"'\\$\\(OS\\)' == 'Windows_NT'\">(.*)</TargetFrameworkProfile>";

	static void Cleanup (StreamReader input, StreamWriter output)
	{
		string line;
		while ((line = input.ReadLine ()) != null) {
			if (Regex.Match (line, nonWindowsRe).Success)
				continue;
			var match = Regex.Match (line, windowsRe);
			if (match.Success)
				output.WriteLine (
					"{0}<TargetFrameworkProfile>{1}</TargetFrameworkProfile>",
					match.Groups [1], match.Groups [2]);
			else
				output.WriteLine (line);
		}
	}
}
