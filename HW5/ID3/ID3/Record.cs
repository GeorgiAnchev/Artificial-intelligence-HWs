using System;
using System.Collections.Generic;
using System.Linq;

namespace ID3
{
    enum OutcomeClass
    {
        NoRecurrence,
        Recurrence
    }

    internal class Record
    {
        public OutcomeClass OutcomeClass { get; set; }

        public string Age { get; set; }

        public string Menopause { get; set; }

        public string TumorSize { get; set; }

        public string InvNodes { get; set; }

        public string NodeCaps { get; set; }

        public string DegMalig { get; set; }

        public string Breast { get; set; }

        public string BreastQuad { get; set; }

        public string Irradiat { get; set; }

        public bool Invalid { get; set; }


        public Record(string row)
        {
            string[] data = row.Split(
                    new[] {',' },
                    StringSplitOptions.RemoveEmptyEntries
                    );

            if ((new List<string>(data)).Any( prop => prop == "?"))
            {
                Invalid = true;
                return;
            }

            OutcomeClass = data[0] == "recurrence-events" ? OutcomeClass.Recurrence : OutcomeClass.NoRecurrence;
            Age = data[1];
            Menopause = data[2];
            TumorSize = data[3];
            InvNodes = data[4];
            NodeCaps = data[5];
            DegMalig = data[6];
            Breast = data[7];
            BreastQuad = data[8];
            Irradiat = data[9];

        }
    }
}