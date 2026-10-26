using System;

namespace ExaminationSystem
{
    #region 4. Answer Class
    public class Answer : ICloneable
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; } = string.Empty;

        public Answer(int id, string text)
        {
            AnswerId = id;
            AnswerText = text;
        }

        public object Clone()
        {
            return new Answer(this.AnswerId, this.AnswerText);
        }

        public override string ToString()
        {
            return $"{AnswerId}. {AnswerText}";
        }
    }
    #endregion

    #region 1, 3, 5. Base Question Class
    public abstract class Question : IComparable<Question>, ICloneable
    {
        public string Header { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public int Mark { get; set; }
        public Answer[] AnswerList { get; set; } = new Answer[0];
        public Answer RightAnswer { get; set; } = null!;
        public Answer UserAnswer { get; set; } = new Answer(0, "");

        protected Question(string header, string body, int mark)
        {
            Header = header;
            Body = body;
            Mark = mark;
        }

        public abstract void Display();

        public int CompareTo(Question? other)
        {
            if (other == null) return 1;
            return this.Mark.CompareTo(other.Mark);
        }

        public object Clone()
        {
            // Simplified deep/shallow copy representation
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{Header} (Mark: {Mark})\n{Body}";
        }
    }
    #endregion

    #region Specific Question Types
    public class TrueFalseQuestion : Question
    {
        public TrueFalseQuestion(string header, string body, int mark, Answer rightAnswer)
            : base(header, body, mark)
        {
            AnswerList = new Answer[]
            {
                new Answer(1, "True"),
                new Answer(2, "False")
            };
            RightAnswer = rightAnswer;
        }

        public override void Display()
        {
            Console.WriteLine($"\n[True/False Question] {Header} | Mark: {Mark}");
            Console.WriteLine(Body);
            foreach (var ans in AnswerList)
            {
                Console.WriteLine($"   {ans}");
            }
        }
    }

    public class McqOneChoiceQuestion : Question
    {
        public McqOneChoiceQuestion(string header, string body, int mark, Answer[] answers, Answer rightAnswer)
            : base(header, body, mark)
        {
            AnswerList = answers;
            RightAnswer = rightAnswer;
        }

        public override void Display()
        {
            Console.WriteLine($"\n[MCQ Question] {Header} | Mark: {Mark}");
            Console.WriteLine(Body);
            foreach (var ans in AnswerList)
            {
                Console.WriteLine($"   {ans}");
            }
        }
    }
    #endregion

    #region 6. Base Exam Class
    public abstract class Exam : ICloneable
    {
        public int TimeOfExam { get; set; } // in minutes
        public int NumberOfQuestions { get; set; }
        public Question[] Questions { get; set; } = new Question[0];
        public Subject AssignedSubject { get; set; } = null!;

        protected Exam(int time, int numberOfQuestions)
        {
            TimeOfExam = time;
            NumberOfQuestions = numberOfQuestions;
        }

        public abstract void ShowExam();

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    #endregion

    #region Exam Types (Practical & Final)
    public class PracticalExam : Exam
    {
        public PracticalExam(int time, int numberOfQuestions) : base(time, numberOfQuestions) { }

        public override void ShowExam()
        {
            Console.WriteLine("\n==================== Practical Exam ====================");
            Console.WriteLine($"Time: {TimeOfExam} minutes | Questions: {NumberOfQuestions}\n");

            for (int i = 0; i < Questions.Length; i++)
            {
                Questions[i].Display();
                int userAnswerId;
                do
                {
                    Console.Write("Enter your answer ID: ");
                } while (!int.TryParse(Console.ReadLine(), out userAnswerId));

                Questions[i].UserAnswer = new Answer(userAnswerId, 
                    Array.Find(Questions[i].AnswerList, a => a.AnswerId == userAnswerId)?.Text ?? "Invalid");
                Console.WriteLine("---------------------------------------------------");
            }

            Console.WriteLine("\n========== Practical Exam Right Answers ==========");
            for (int i = 0; i < Questions.Length; i++)
            {
                Console.WriteLine($"Q{i + 1}: {Questions[i].Body}");
                Console.WriteLine($"Right Answer: {Questions[i].RightAnswer.AnswerText}\n");
            }
        }
    }

    public class FinalExam : Exam
    {
        public FinalExam(int time, int numberOfQuestions) : base(time, numberOfQuestions) { }

        public override void ShowExam()
        {
            Console.WriteLine("\n====================== Final Exam ======================");
            Console.WriteLine($"Time: {TimeOfExam} minutes | Questions: {NumberOfQuestions}\n");

            int totalMarks = 0;
            int obtainedMarks = 0;

            for (int i = 0; i < Questions.Length; i++)
            {
                Questions[i].Display();
                int userAnswerId;
                do
                {
                    Console.Write("Enter your answer ID: ");
                } while (!int.TryParse(Console.ReadLine(), out userAnswerId));

                Questions[i].UserAnswer = new Answer(userAnswerId,
                    Array.Find(Questions[i].AnswerList, a => a.AnswerId == userAnswerId)?.Text ?? "Invalid");
                
                totalMarks += Questions[i].Mark;
                Console.WriteLine("---------------------------------------------------");
            }

            Console.WriteLine("\n==================== Exam Results ====================");
            for (int i = 0; i < Questions.Length; i++)
            {
                bool isCorrect = Questions[i].UserAnswer.AnswerId == Questions[i].RightAnswer.AnswerId;
                if (isCorrect) obtainedMarks += Questions[i].Mark;

                Console.WriteLine($"Q{i + 1}: {Questions[i].Body}");
                Console.WriteLine($"Your Answer: {Questions[i].UserAnswer.AnswerText}");
                Console.WriteLine($"Right Answer: {Questions[i].RightAnswer.AnswerText}");
                Console.WriteLine($"Status: {(isCorrect ? "Correct ✓" : "Wrong ✗")}\n");
            }

            Console.WriteLine($"Your Total Grade: {obtainedMarks} / {totalMarks}");
            Console.WriteLine("=======================================================");
        }
    }
    #endregion

    #region 7. Subject Class
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public Exam SubjectExam { get; set; } = null!;

        public Subject(int id, string name)
        {
            SubjectId = id;
            SubjectName = name;
        }

        public void CreateExam()
        {
            int examType, time, numQuestions;

            do
            {
                Console.Write("Enter Exam Type (1 for Practical, 2 for Final): ");
            } while (!int.TryParse(Console.ReadLine(), out examType) || (examType != 1 && examType != 2));

            do
            {
                Console.Write("Enter Exam Time in minutes: ");
            } while (!int.TryParse(Console.ReadLine(), out time) || time <= 0);

            do
            {
                Console.Write("Enter Number of Questions: ");
            } while (!int.TryParse(Console.ReadLine(), out numQuestions) || numQuestions <= 0);

            if (examType == 1)
            {
                SubjectExam = new PracticalExam(time, numQuestions);
            }
            else
            {
                SubjectExam = new FinalExam(time, numQuestions);
            }

            SubjectExam.AssignedSubject = this;
            SubjectExam.Questions = new Question[numQuestions];

            for (int i = 0; i < numQuestions; i++)
            {
                Console.WriteLine($"\n--- Configuring Question {i + 1} ---");
                int qType;

                if (examType == 1)
                {
                    qType = 1; // Practical only has MCQ
                }
                else
                {
                    do
                    {
                        Console.Write("Choose Question Type (1 for True/False, 2 for MCQ): ");
                    } while (!int.TryParse(Console.ReadLine(), out qType) || (qType != 1 && qType != 2));
                }

                Console.Write("Enter Question Header: ");
                string header = Console.ReadLine() ?? "";

                Console.Write("Enter Question Body: ");
                string body = Console.ReadLine() ?? "";

                int mark;
                do
                {
                    Console.Write("Enter Question Mark: ");
                } while (!int.TryParse(Console.ReadLine(), out mark) || mark <= 0);

                if (qType == 1 && examType == 2)
                {
                    // True / False
                    int rightAnsChoice;
                    do
                    {
                        Console.Write("Enter Right Answer (1 for True, 2 for False): ");
                    } while (!int.TryParse(Console.ReadLine(), out rightAnsChoice) || (rightAnsChoice != 1 && rightAnsChoice != 2));

                    Answer rightAns = rightAnsChoice == 1 ? new Answer(1, "True") : new Answer(2, "False");
                    SubjectExam.Questions[i] = new TrueFalseQuestion(header, body, mark, rightAns);
                }
                else
                {
                    // MCQ
                    Console.WriteLine("Enter MCQ Choices:");
                    Answer[] answers = new Answer[3];
                    for (int j = 0; j < 3; j++)
                    {
                        Console.Write($"Enter Choice {j + 1}: ");
                        string choiceText = Console.ReadLine() ?? "";
                        answers[j] = new Answer(j + 1, choiceText);
                    }

                    int rightAnsId;
                    do
                    {
                        Console.Write("Enter Right Answer ID (1, 2, or 3): ");
                    } while (!int.TryParse(Console.ReadLine(), out rightAnsId) || rightAnsId < 1 || rightAnsId > 3);

                    Answer rightAns = answers[rightAnsId - 1];
                    SubjectExam.Questions[i] = new McqOneChoiceQuestion(header, body, mark, answers, rightAns);
                }
            }
        }
    }
    #endregion

    #region 10. Program Main
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("          WELCOME TO EXAMINATION SYSTEM           ");
            Console.WriteLine("==================================================");

            // 10. Declare a subject object
            Subject csharpSubject = new Subject(101, "C# Programming");

            // Create exam for the subject
            csharpSubject.CreateExam();

            Console.Clear();
            Console.WriteLine($"Subject: {csharpSubject.SubjectName}");
            Console.Write("Do you want to start the exam now? (y/n): ");
            char start = Console.ReadKey().KeyChar;

            if (start == 'y' || start == 'Y')
            {
                Console.Clear();
                csharpSubject.SubjectExam.ShowExam();
            }

            Console.WriteLine("\nThank you! Press any key to exit.");
            Console.ReadKey();
        }
    }
    #endregion
}