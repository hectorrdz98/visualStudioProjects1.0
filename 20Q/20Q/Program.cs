
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20Q
{
    class Program
    {
        static List<Object> posibles;
        static List<Question> questionsPassed;

        static void Main(string[] args)
        {
            posibles = new List<Object>();
            questionsPassed = new List<Question>();
            Question[] questions = dbSeeder();
            int total = 0;

            // Print objects
            Console.WriteLine("\nObjects:\n");
            Array.ForEach(posibles.Select(x => x.content).ToArray(), Console.WriteLine);

            IEnumerable<Question> scoreQuery =
                from question in questions
                orderby question.searchAndCount(posibles) ascending
                select question;

            while (scoreQuery.Count() > 0)
            {
                Console.WriteLine("\nThe ordered list of questions that contain the posible elements is: " + scoreQuery.Count());
                foreach (Question question in scoreQuery)
                    Console.WriteLine(question.content + " -> " + question.searchAndCount(posibles) + " euristic points");

                Question questionTop = scoreQuery.First();

                Console.Write("\n" + questionTop.content + " ");
                bool answer = Console.ReadLine().ToUpper() == "YES" ? true : false;

                questionsPassed.Add(questionTop);

                Object[] acceptedObjects = answer ? questionTop.yes.ToArray() : questionTop.no.ToArray();
                Object[] deletedObjects = answer ? questionTop.no.ToArray() : questionTop.yes.ToArray();

                for (int i = 0; i < deletedObjects.Length; i++)
                    if (posibles.Contains(deletedObjects[i])) posibles.Remove(deletedObjects[i]);
                for (int i = 0; i < acceptedObjects.Length; i++)
                    acceptedObjects[i].value += 1;
                for (int i = 0; i < posibles.Count(); i++)
                    if (!Array.Exists(acceptedObjects, e => e == posibles[i]))
                        if (!Array.Exists(deletedObjects, e => e == posibles[i])) posibles[i].value += 0.4;

                Console.WriteLine("\nPosibles: ");
                for (int i = 0; i < posibles.Count(); i++)
                    Console.WriteLine(posibles[i].content + ": " + posibles[i].value);

                questions = (questions.ToList().Except(questionsPassed)).ToArray();

                scoreQuery =
                    from question in questions 
                    orderby question.searchAndCount(posibles) ascending
                    select question;
                total += 1;
            }

            if (total > 0)
            {
                IEnumerable<Object> objectsQuery = 
                    from posible in posibles 
                    orderby posible.value descending 
                    select posible; 

                Console.WriteLine("\n\n----- Winners -----");

                foreach (Object obj in objectsQuery)
                {
                    Console.WriteLine(obj.content + ": " + (obj.value / total * 100) + "%");
                }

                Console.WriteLine("-------------------\n");
            }
            else { Console.WriteLine("No results"); }
        }

        static Question[] dbSeeder()
        {
            // Manzana, Fresa, Naranja, Zanahoria, Brócoli, Tomate

            Object manzana = new Object("Manzana");
            Object fresa = new Object("Fresa");
            Object naranja = new Object("Naranja");
            Object zanahoria = new Object("Zanahoria");
            Object brocoli = new Object("Brócoli");
            Object tomate = new Object("Tomate");

            Object perro = new Object("Perro");
            Object gato = new Object("Gato");
            Object puerco = new Object("Puerco");
            Object gallina = new Object("Gallina");
            Object caballo = new Object("Caballo");

            Object arbol = new Object("Arbol");

            Object auto = new Object("Auto");
            Object computadora = new Object("Computadora");
            Object telefono = new Object("Telefono");
            Object mochila = new Object("Mochila");
            Object casa = new Object("Casa");

            posibles.Add(manzana);
            posibles.Add(fresa);
            posibles.Add(naranja);
            posibles.Add(zanahoria);
            posibles.Add(brocoli);
            posibles.Add(tomate);
            posibles.Add(perro);
            posibles.Add(gato);
            posibles.Add(puerco);
            posibles.Add(gallina);
            posibles.Add(caballo);
            posibles.Add(arbol);
            posibles.Add(auto);
            posibles.Add(computadora);
            posibles.Add(telefono);
            posibles.Add(mochila);
            posibles.Add(casa);

            Question[] questions = {
                new Question("¿Es una fruta?"),
                new Question("¿Es color rojo?"),
                new Question("¿Es color naranja?"),
                new Question("¿Es un ser vivo?"),
                new Question("¿Es un animal?"),
                new Question("¿Es comida?"),
                new Question("¿Tiene pelo?"),
                new Question("¿Tiene plumas?"),
                new Question("¿Es comida?")
            };

            questions[0].yes.Add(manzana);
            questions[0].yes.Add(fresa);
            questions[0].yes.Add(naranja);
            questions[0].no.Add(zanahoria);
            questions[0].no.Add(brocoli);
            questions[0].no.Add(tomate);

            questions[1].yes.Add(manzana);
            questions[1].yes.Add(tomate);
            questions[1].yes.Add(fresa);
            questions[1].no.Add(zanahoria);
            questions[1].no.Add(brocoli);

            questions[2].yes.Add(zanahoria);
            questions[2].yes.Add(naranja);
            questions[2].no.Add(fresa);
            questions[2].no.Add(brocoli);

            questions[3].yes.Add(perro);
            questions[3].yes.Add(gato);
            questions[3].yes.Add(puerco);
            questions[3].yes.Add(gallina);
            questions[3].yes.Add(caballo);
            questions[3].yes.Add(arbol);
            questions[3].no.Add(manzana);
            questions[3].no.Add(fresa);
            questions[3].no.Add(naranja);
            questions[3].no.Add(zanahoria);
            questions[3].no.Add(brocoli);
            questions[3].no.Add(tomate);
            questions[3].no.Add(auto);
            questions[3].no.Add(computadora);
            questions[3].no.Add(telefono);
            questions[3].no.Add(mochila);
            questions[3].no.Add(casa);

            questions[4].yes.Add(perro);
            questions[4].yes.Add(gato);
            questions[4].yes.Add(puerco);
            questions[4].yes.Add(gallina);
            questions[4].yes.Add(caballo);
            questions[4].no.Add(arbol);

            questions[5].yes.Add(manzana);
            questions[5].yes.Add(fresa);
            questions[5].yes.Add(naranja);
            questions[5].yes.Add(zanahoria);
            questions[5].yes.Add(brocoli);
            questions[5].yes.Add(tomate);
            questions[5].no.Add(auto);
            questions[5].no.Add(computadora);
            questions[5].no.Add(telefono);
            questions[5].no.Add(mochila);
            questions[5].no.Add(casa);

            return questions;
        }
    }
}
