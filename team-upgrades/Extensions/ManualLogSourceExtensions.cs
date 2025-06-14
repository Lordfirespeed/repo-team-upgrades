using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using HarmonyLib;

namespace TeamUpgrades.Extensions;

public static class ManualLogSourceExtensions
{
    class CodeInstructionFormatter(int instructionCount)
    {
        private int _instructionIndexPadLength = instructionCount.ToString().Length;

        public string Format(CodeInstruction instruction, int index)
            => $"    IL_{index.ToString().PadLeft(_instructionIndexPadLength, '0')}: {instruction}";
    }

    public static void LogDebugInstructionsFrom(this ManualLogSource source, CodeMatcher matcher)
    {
        var methodName = new StackTrace().GetFrame(1).GetMethod().Name;

        var instructionFormatter = new CodeInstructionFormatter(matcher.Length);
        var builder = new StringBuilder($"'{methodName}' Matcher Instructions:\n")
            .AppendLine(
                String.Join(
                    "\n",
                    matcher
                        .InstructionEnumeration()
                        .Select(instructionFormatter.Format)
                )
            )
            .AppendLine("End of matcher instructions.");

        source.LogDebug(builder.ToString());
    }
}
