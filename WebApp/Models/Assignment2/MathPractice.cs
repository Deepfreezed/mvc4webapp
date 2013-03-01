using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Assignment2
{
	public class MathPractice
	{
		public int ID { get; set; }
				
		public List<Question> Questions { get; set; }

		public Question CurrentQuestion
		{ 
			get
			{				
				if(Questions != null || Questions.Count > 0)
				{
					return Questions.FirstOrDefault(q => q.Answer == -1);
				}
				else
				{
					return null;
				}
			}
		}

		public int ProblemsCorrect
		{
			get
			{
				int score = 0;

				if(Questions != null || Questions.Count > 0)
				{
					score = Questions.Where(q => q.Answer == q.Solution).Count();
				}

				return score;
			}
		}

		public int ProblemsAnswered
		{
			get
			{
				int answered = 0;

				if(Questions != null || Questions.Count > 0)
				{
					answered = Questions.Where(q => q.Answer != -1).Count();
				}

				return answered;
			}
		}

		public int ProblemsWrong
		{
			get
			{
				int wrong = 0;

				if(Questions != null || Questions.Count > 0)
				{
					wrong = Questions.Where(q => q.Answer != q.Solution && q.Answer != -1).Count();
				}

				return wrong;
			}
		}

		public string Rating 
		{ 
			get
			{
				string rating = string.Empty;

				if(ProblemsAnswered > 0)
				{
					decimal ratingPercent = (Convert.ToDecimal(ProblemsCorrect) / Convert.ToDecimal(ProblemsAnswered)) * 100;

					if(ratingPercent > 95)
					{
						rating = "A";
					}
					else if(ratingPercent > 85)
					{
						rating = "B+";
					}
					else if(ratingPercent > 80)
					{
						rating = "B";
					}
					else if(ratingPercent > 70)
					{
						rating = "C+";
					}
					else if(ratingPercent > 60)
					{
						rating = "C";
					}
					else if(ratingPercent > 50)
					{
						rating = "D+";
					}
					else if(ratingPercent > 40)
					{
						rating = "D";
					}
					else
					{
						rating = "F";
					}
				}

				return rating;
			}
		}

		public void CreateQuestions()
		{
			if(Questions == null || Questions.Count == 0)
			{
				Random random = new Random();
				Questions = new List<Question>();

				for(int i = 0; i < 10; i++)
				{
					Question question = new Question();
					question.FirstNumber = random.Next(1, 30);
					question.SecondNumber = random.Next(1, (30 - question.FirstNumber) + 1);

					Questions.Add(question);
				}
			}
		}
	}
}