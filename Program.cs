using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace caAssignment1
{
   
    public class Program
    {
        public class student
        {
            public string name { get; set; }
            public int rating { get; set; }
            public int assigned_notebook { get; set; }

            public student(string Name, int Rating, int Assigned_notebook)
            {
                name = Name;
                rating = Rating;
                assigned_notebook = Assigned_notebook;
            }

        }

    static void Main(string[] args)
        {
            
            notebookAssignment("Class1_ratings.csv");
            notebookAssignment("Class1_ratings_invalidheader.csv");
            notebookAssignment("File_Not_Exist.csv");
            notebookAssignment("Class1_ratings_invalid_col_number.csv");
            notebookAssignment("Class1_ratings_invalid_student_name.csv");
            notebookAssignment("Class1_ratings_invalid_student_col.csv");
            notebookAssignment("Class1_ratings_invalid_rating.csv");


        }

        public static string notebookAssignment(string pInputFile)
        {
            string result = "";
            int temp_rating = -1;
            int temp_assigned_nb = -1;
            int new_rating = -1;
            string[] temp_Arr;
            string currentLine;
            string ouput_file = "";
            List<student> finalList = new List<student>();

            logging("N", "Program Started~");

            // use Reflection to get the exxecuting assembl path  
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // Pass that the Path.GetDirectoryName  
            pInputFile = Path.GetDirectoryName(path) + "/" + pInputFile;
            ouput_file = Path.GetDirectoryName(path) + "/" + "Output_" + DateTime.Now.ToString("yyyy-MM-ddHHmmss") + ".csv";
            if (!File.Exists(pInputFile))
            {
                result = "Input file not existed!";
                Console.WriteLine(result);
                logging("E", result);
                return result;
            }

            
            using (StreamReader sr = new StreamReader(pInputFile))
            {
                
                //For the first line to validate the file headers
                currentLine = sr.ReadLine();
                temp_Arr = currentLine.Split(",");
                
                if (temp_Arr.Length <3)
                {
                     
                    result = "Invalid Header Number!";
                    Console.WriteLine(result);
                    logging("E", result);
                    return result;

                }
                else if (temp_Arr[0].Trim() != "Name" || temp_Arr[1].Trim() != "Rating")
                {
                     
                    result = "Invalid Header Name!";
                    Console.WriteLine(result);
                    logging("E", result);
                    return result;

                }


                // currentLine will be null when the StreamReader reaches the end of file
                while ((currentLine = sr.ReadLine()) != null)
                {
                     temp_Arr = currentLine.Split(",");
                    if (temp_Arr.Length < 3)
                    {//check if the number of data extracted is correct
                     
                        result = "Invalid Column Amount Of Student!";
                        Console.WriteLine(result);
                        logging("E", result);
                        return result;

                    }

                    if (string.IsNullOrEmpty(temp_Arr[0].Trim()))
                    {//check if student name is input
                         
                        result = "Student Name empty!";
                        Console.WriteLine(result);
                        logging("E", result);
                        return result;

                    }

                    if (int.TryParse(temp_Arr[1], out new_rating))
                    {
                        //if it is the first student, inital the value
                        if (temp_rating == -1 && temp_assigned_nb == -1)
                        {
                            temp_rating = new_rating;
                            temp_assigned_nb = 1;

                            student student = new student(temp_Arr[0], temp_rating, temp_assigned_nb);
                            finalList.Add(student);
                        }
                        else
                        {
                            //if the rating of next student higher than previous one, increase the number of assigned notebook
                            if (temp_rating < new_rating)
                            {
                                temp_assigned_nb = temp_assigned_nb + 1;
                            }
                            else if (temp_rating == new_rating)
                            {

                            }//if the rating of next student lower than previous one, reset the number of assigned notebook to 1
                            else if (temp_rating > new_rating)
                            {                             
                                temp_assigned_nb =  1;
                            }
                            //create a student object to store the data and add it to a list
                            temp_rating = new_rating;
                            student student = new student(temp_Arr[0], temp_rating, temp_assigned_nb);
                            finalList.Add(student);
                        }

                    }
                    else
                    {
                         
                        result = "Invalid Student Rating Type";
                        Console.WriteLine(result);
                        logging("E", result);
                        return result;

                    }

                }
            }

            result = generateOutput(ouput_file, finalList);

            if (result == "")
            {
                result = "Generation completed!";
                logging("N", result);
                return result;

            }
            else
            {
                logging("E", result);
                return result;
            }

        }

        static string generateOutput(string outputName,List<student> studentsList)
        {
            try
            {
                //check and create the output file
                if (!File.Exists(outputName))
                {
                    FileStream fs = File.Create(outputName);
                    fs.Close();
                }
                else
                {
                    File.Delete(outputName);
                    FileStream fs = File.Create(outputName);
                    fs.Close();
                }

                var csv = new StringBuilder();

                //create the file header
                string first = "Name";
                string second = "Notebooks";
                var newLine = string.Format("{0}, {1}, {2}", first, second, Environment.NewLine);
                csv.Append(newLine);
                //iterate all the student class in the list and append the string
                foreach (student student1 in studentsList)
                {
                    first = student1.name;
                    second = student1.assigned_notebook.ToString();
                    newLine = string.Format("{0}, {1}, {2}", first, second, Environment.NewLine);
                    string result = student1.name + ", " + student1.assigned_notebook + ", ";
                    logging("N", result);
                    csv.Append(newLine);
                }
                //get the total by using LINQ
                first = "Total";
                second = studentsList.Sum(s => s.assigned_notebook).ToString();
                newLine = string.Format("{0}, {1}, {2}", first, second, Environment.NewLine);
                logging("N", newLine);
                csv.Append(newLine);

                //Append the content to the csv file
                File.WriteAllText(outputName, csv.ToString());

                return "";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logging("E", ex.Message + " - " + ex.StackTrace);
                return ex.StackTrace;
            }

            
             

        }

        static void logging(string type, string content)
        {
            string fileName = "log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = File.Create(fileName);
                fs.Close();
           
            }

            using (StreamWriter w = File.AppendText(fileName))
            {
                if (type == "E")
                {
                    w.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - Error: " + content);
                     

                }
                else
                {
                    w.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +" - "+ content);
             
                }
            }

            

            
        }

    }
}
