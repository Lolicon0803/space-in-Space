using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Assets.Scripts.TextWriter
{
    public class Paragraph
    {
        public int textBoxIndex;
        public string speaker;
        public string text;
        public Paragraph(int textBoxIndex, string speaker, string text)
        {
            this.textBoxIndex = textBoxIndex;
            this.speaker = speaker;
            this.text = text;
        }
        public string GetText()
        {
            return text;
        }
    }
    public class Story : List<Paragraph>
    {
        public Story()
        {
        }
        //public new object this[int i]
        //{
        //    get { return this[i]; }
        //}
    }


}
