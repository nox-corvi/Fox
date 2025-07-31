using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Force
{
    public record LzToken
    {
        public byte? Literal { get; init; }    // gesetzt, wenn Literal
        public int? Distance { get; init; }    // gesetzt, wenn Match
        public int? Length { get; init; }      // gesetzt, wenn Match

        public bool IsLiteral => Literal.HasValue;
        public bool IsMatch => Distance.HasValue && Length.HasValue;
    }

    public class TinyCommons
    {
    }
}
