#region Command Line Application Template - Tristan Phillips - PLEASE LEAVE THIS HERE
/*
Command Line Application Template
Tristan Phillips - Oct 2007
This application is provided "as is" and comes with ABSOLUTELY NO WARRANTY, 
to the extent permitted by applicable law.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace yCopy
{

	/// <summary>
	/// </summary>
	class Program
	{

		const string APP_NAME = "yCopy";
		const string APP_VERSION = "0.3";

		static string Src = string.Empty;
		static string Dest = string.Empty;
		static bool Verify = false;
		static bool Verbose = false;
		static DateTime FromDate = DateTime.MinValue;
		static DateTime ToDate = DateTime.MinValue;
		static bool DateFilter = false;
		static bool Recursive = false;
		static bool Overwrite = false;
		static bool DeleteSrc = false;
		static string IP = "";

		static int Copied = 0;

		static void Main(string[] args)
		{
			Console.WriteLine("\n" + APP_NAME + " v" + APP_VERSION + " - Tristan Phillips\n");
			if (!ParseArgs(args)) { ShowUsage(); return; }
			if (IP == "" || IPMatchesHost(IP))
			{
				ProcessDirectory(Src, Dest);
				Console.WriteLine("\nCopied " + Copied + " file(s)");
			}
			else
			{
				Console.WriteLine("Host IP does not match.  Aborting.")	;
			}
#if DEBUG
			Console.ReadLine();
#endif
		}

		static bool IPMatchesHost(string IP)
		{
			foreach (System.Net.IPAddress ip in System.Net.Dns.GetHostAddresses(Environment.MachineName))
			{
				if (ip.ToString() == IP) { return true; }
			}
			return false;
		}

		static void ProcessDirectory(string Src, string Dest)
		{
			// Check the src exists
			if (Directory.Exists(Src))
			{
				// Get the directory info
				DirectoryInfo SrcD = new DirectoryInfo(Src);
				// Check the dest exists, if not, make it. 
				if (!Directory.Exists(Dest)){ Directory.CreateDirectory(Dest); }
				DirectoryInfo DstD = new DirectoryInfo(Dest);
				// Enumerate and process the files.
				foreach (FileInfo File in SrcD.GetFiles())
				{
					if (!DateFilter || (File.CreationTime >= FromDate && File.CreationTime < ToDate))
					{
						if (Verbose) { Console.WriteLine("Copying " + File.FullName); }
						Copied++;
						string FileDest = DstD.FullName + "\\" + File.Name;
						try
						{
							// Copy
							File.CopyTo(FileDest, Overwrite);
							// Verify
							bool Verification = false;
							if (Verify)
							{
								if (Verbose) { Console.WriteLine("Verifying " + File.FullName); }
								Verification = VerifyFile(File.FullName, FileDest);
								if (!Verification)
								{
									Console.WriteLine("WARNING File verification filed for " + FileDest);
								}
							}
							// Delete
							if (DeleteSrc)
							{
								if (Verbose) { Console.WriteLine("Deleting " + File.FullName); }
								if (Verify && !Verification)
								{
									Console.WriteLine("WARNING File deletion skipped for " + File.FullName);
								}
								else
								{
									try
									{
										File.Delete();
									}
									catch (Exception e)
									{
										Console.WriteLine(e.Message);
									}
								}
							}
						}
						catch (Exception e)
						{
							Console.WriteLine(e.Message);
						}
					}
				}
				// Recurse if needed
				if (Recursive)
				{
					foreach (DirectoryInfo Dir in SrcD.GetDirectories())
					{
						ProcessDirectory(Dir.FullName, DstD.FullName + "\\" + Dir);
					}
				}
			}
		}

		static bool VerifyFile(string src, string dst)
		{
			// Check for CRC errors by reading the whole file
			try
			{
				FileStream fs = new FileStream(dst, FileMode.Open);
				int Read = 0;
				int TotalRead = 0;
				int Block = 102400;
				long Length = (int)new FileInfo(dst).Length;
				byte[] buff = new byte[Length];
				do
				{
					if ((Length - TotalRead) < Block) { Block = (int)(Length - TotalRead); }
					Read = fs.Read(buff, TotalRead, Block);
					TotalRead += Read;
				}
				while (Read != 0 && TotalRead != Length);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
			// Check the file lengths
			return (new FileInfo(src).Length == new FileInfo(dst).Length);
		}

		/// <summary>
		/// ParseArgs parses user input into the parameter variables defined.
		/// It should also validate that all arguments required are provided.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		static bool ParseArgs(string[] args)
		{
			try
			{
				if (args.Length < 2) { return false; }
				Src = args[0];
				Dest = args[1];
				for (int x = 2; x < args.Length; x++)
				{
					if (args[x] == "-?") { return false; }
					if (args[x] == "-f") { FromDate = DateTime.Parse(args[x + 1]); }
					if (args[x] == "-t") { ToDate = DateTime.Parse(args[x + 1]); }
					if (args[x] == "-i") { IP = args[x + 1]; }
					if (args[x] == "-T") 
					{
						FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
						ToDate = new DateTime(DateTime.Today.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day);
					}
					if (args[x] == "-Y")
					{
						FromDate = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day);
						ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
					}
					if (args[x] == "-v") { Verify = true; }
					if (args[x] == "-r") { Recursive = true; }
					if (args[x] == "-o") { Overwrite = true; }
					if (args[x] == "-d") { DeleteSrc = true; }
					if (args[x] == "-V") { Verbose = true; }
				}
			}
			catch
			{
				return false;
			}
			if (FromDate != DateTime.MinValue && ToDate == DateTime.MinValue)
			{
				Console.WriteLine("A to date must be specified with a from date.");
			}
			if (ToDate != DateTime.MinValue && FromDate == DateTime.MinValue)
			{
				Console.WriteLine("A from date must be specified with a to date.");
			}
			if (Src == String.Empty || Dest == String.Empty) { return false; }
			if (FromDate != DateTime.MinValue || ToDate != DateTime.MinValue) { DateFilter = true; }
			return true;
		}

		/// <summary>
		/// ShowUsage is responsible for the console output displayed to a user if the arguments
		/// required are not provided, or if they ask for help.
		/// </summary>
		static void ShowUsage()
		{
			Console.WriteLine("\nusage: yCopy.exe Src Dest [-f -t -T -Y -v -r -V -i]\n");
			Console.WriteLine("\t-? Show this help.");
			Console.WriteLine("\t-f <Date> From date - copy files older thn this date.");
			Console.WriteLine("\t-t <Date> To date - copy files younger than this date.");
			Console.WriteLine("\t-T Set the from and to date filters to include files created today.");
			Console.WriteLine("\t-Y Set the from and to date filters to include files created yesterday.");
			Console.WriteLine("\t-v Verify the files after copying.");
			Console.WriteLine("\t-r Recursive copy.");
			Console.WriteLine("\t-o Overwrite destination if it already exists (default no).");
			Console.WriteLine("\t-d Delete src after copying (skipped if verification fails).");
			Console.WriteLine("\t-V Verbose.");
			Console.WriteLine("\t-i <IP> only perform the copy if the hosts IP is <IP>.");
			Console.WriteLine("\n\tDates are expressed as regular strings, for example:\n" +
				"\n\tyCopy c:\\bla c:\\tmp -f \"10-jan-08 12:45\" -t \"20-jan-08\".\n" +
				"\n\tThis copies all files in c:\\bla to c:\\tmp where the files create" +
				"\n\tdate is older than the 10th of Jan at 12:45, and younger" +
				"\n\tthan the 20th Jan 00:00 hours." +
				"\n\n\tIf times are omitted from dates, 00:00:00 is assumed." +
				"\n\tArguments containing spaces MUST be quoted.");
		}

	}
}
