﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MPQNav.Util.ADTParser {
	internal class MMDXChunkParser : ChunkParser<string[]> {
		public MMDXChunkParser(BinaryReader br, long pAbsoluteStart)
			: base("MMDX",br, pAbsoluteStart) {
		}

		/// <summary>
		/// Parse MMDX element from file stream
		/// </summary>
		public override string[] Parse() {
			Reader.BaseStream.Position = AbsoluteStart;

			var result = new List<string>();
			long end = AbsoluteStart + Size;
			while(Reader.BaseStream.Position < end) {
				result.Add(Reader.ReadCString());
			}
			return result.ToArray();
		}
	}
}