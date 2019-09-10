using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20Q
{
    class Question
    {
        public string content;
        public List<Object> yes;
        public List<Object> no;

        public Question(string content)
        {
            this.content = content;
            this.yes = new List<Object>();
            this.no = new List<Object>();
        }

        public int searchAndCount(List<Object> perfectForQuestion)
        {
            int yesCont = 0;
            int noCont = 0;
            int extraCont = 0;
            List<Object> alreadyPassed = new List<Object>();
            for (int i = 0; i < this.yes.Count; i++)
                if (perfectForQuestion.Any(p => p.content == this.yes[i].content))
                {
                    yesCont++;
                    alreadyPassed.Add(this.yes[i]);
                }
            for (int i = 0; i < this.no.Count; i++)
                if (perfectForQuestion.Any(p => p.content == this.no[i].content))
                {
                    noCont++;
                    alreadyPassed.Add(this.no[i]);
                }
            for (int i = 0; i < perfectForQuestion.Count(); i++)
                if (!alreadyPassed.Contains(perfectForQuestion[i])) extraCont++;
            return Math.Abs(yesCont-noCont) + extraCont;
        }
    }
}
