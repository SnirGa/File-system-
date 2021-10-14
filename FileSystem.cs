using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace BGUFS
{
    class FileSystem
    {
        public string[,] Header2 { get; set; }
        public Byte[] Content { get; set; }
        //public String contentAsString { get; set; }
    public String Name { get; set; }
        public int HeaderNextIndex { get; set; }
        public int ContentNextIndex { get; set; }


        public FileSystem(String name)
        {
            this.Name = name;
            //this.Header = new FileObject[1000];
            this.Header2= new string[1000,6];
            this.Content = new Byte[100000000];
            this.HeaderNextIndex = 0;
            this.ContentNextIndex = 0;
        }

        public bool addFile(String fileName)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Header2[i,0] != null)
                {
                    if (Header2[i,0].Equals(fileName) && Header2[i,5]=="not deleted")
                    {
                        return false;
                    }
                }
            }
            Byte[] newFileBytes = File.ReadAllBytes(fileName);
            String S = Convert.ToBase64String(newFileBytes);
            /* FileObject f = new FileObject(fileName,"origin");
             f.setLength(newFileBytes.Length);
             f.setLocation(ContentNextIndex);
             Header[HeaderNextIndex] = f;
             HeaderNextIndex++;*/
            Header2[HeaderNextIndex, 0] = fileName;
            Header2[HeaderNextIndex, 1] = ContentNextIndex.ToString();
            Header2[HeaderNextIndex, 2] = newFileBytes.Length.ToString();
            FileInfo fi = new FileInfo(fileName);
            Header2[HeaderNextIndex, 3] = fi.CreationTime.ToString();
            Header2[HeaderNextIndex, 4] = "regular";
            Header2[HeaderNextIndex, 5] = "not deleted";


            HeaderNextIndex++;


            if (ContentNextIndex + newFileBytes.Length > Content.Length)
            {
                Byte[] temp2 = new Byte[Content.Length + newFileBytes.Length];
                for (int i = 0; i < Content.Length; i++)
                {
                    temp2[i] = Content[i];
                }
                Content = temp2;

            }

            for (int i = 0; i < newFileBytes.Length; i++)
            {
                Content[ContentNextIndex] = newFileBytes[i];
                ContentNextIndex++;
            }
            return true;
        }

        /*public bool addFile2(String fileName)
        {
            for (int i = 0; i < Header2.Length; i++)
            {
                if (Header[i] != null)
                {
                    if (Header2[i][0].Equals(fileName))
                    {
                        return false;
                    }
                }
            }

            Header2[HeaderNextIndex][0] = fileName;

         }*/

        public bool removeFile(String fileName)
        {
            for (int i = 0; i <1000; i++)
            {
                if (Header2[i, 0] != null)
                {
                    if (Header2[i, 0].Equals(fileName) && Header2[i, 5] == "not deleted")
                    {
                        Header2[i, 5] = "deleted";
                        for (int j = 0; j < 1000; j++)
                        {
                            if (Header2[j, 0] != null)
                            {
                                if (Header2[j, 1].Equals(Header2[i, 1]) && Header2[j,4].Equals("link"))
                                {
                                    Header2[j, 5] = "deleted";
                                }
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public void rename(String fileName, String NewFileName)
        {
            bool found = false;
            for (int i = 0; i < 1000; i++)
            {
                if (Header2[i, 0] != null)
                {
                    if (Header2[i, 0].Equals(NewFileName) && Header2[i, 5] == "not deleted")
                    {
                        Console.WriteLine("file {0} already exist", NewFileName);
                        return;
                    }
                }

            }
            for (int i = 0; i < 1000; i++)
            {
                if (Header2[i, 0] != null)
                {
                    if (Header2[i, 0].Equals(fileName) && Header2[i, 5] == "not deleted")
                    {
                        found = true;
                        Header2[i, 0] = NewFileName;
                        File.Move(fileName, NewFileName);
                        break;
                    }
                }   
            }
            if (!found)
            {
                Console.WriteLine("file does not exist");
            }

        }

        public void optimize()
        {
            cleanHeader();
            clearContent();

        }

        public bool extract(String fileName, String extractedFileName)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Header2[i, 0] != null)
                {
                    if (Header2[i, 0].Equals(fileName) && Header2[i, 5] == "not deleted")
                    {
                        int location = Int32.Parse(Header2[i, 1]);
                        int size = Int32.Parse(Header2[i, 2]);
                        Byte[] bytes = new Byte[size];
                        for(int j = 0; j < size; j++)
                        {
                            bytes[j] = Content[location];
                            location++;
                        }
                        File.WriteAllBytes(extractedFileName, bytes);
                        
                        return true;
                    }
                }
            }
            return false;
        }

        public void dir()
        {
            String str = "";
            bool first = true;
            for (int i = 0; i < 1000; i++)
            {
                if (Header2[i, 0] != null)
                {
                    if (Header2[i, 5].Equals("not deleted"))
                    {
                        if (!first)
                        {
                            str += "\n";
                        }
                        first = false;
                        
                        str += Header2[i, 0];
                        str += ",";
                        str += Header2[i, 2];
                        str += ",";
                        str += Header2[i, 3];
                        str += ",";
                        str += Header2[i, 4]; //need to add link
                        //str += "\n";
                        if (Header2[i, 4].Equals("link"))
                        {
                            for (int j = 0; j < 1000; j++)
                            {
                                if (Header2[j, 0] != null)
                                {
                                    if (Header2[j, 1].Equals(Header2[i, 1]) && Header2[j, 4].Equals("regular") && Header2[j,5].Equals("not deleted"))
                                    {
                                        str += ",";
                                        str += Header2[j, 0];
                                        
                                    }
                                }
                            }
                        }
                    }

                }
            }
            if (str == "")
            {
                return;
            }
            Console.WriteLine(str);
        }

        public void hash(String fileName)
        {
            String str = "";
            for (int i = 0; i < 1000; i++)
            {
                if (Header2[i, 0] != null)
                {
                    if (Header2[i, 0].Equals(fileName) && Header2[i, 5].Equals("not deleted"))
                    {
                        str = checkMD5(fileName);
                        Console.WriteLine(str);
                        return;
                    }
                }

            }
            Console.WriteLine("file does not exist");
        }



        

        public void sortAB()
        {
            String[,] temp=Header2.OrderBy(x => x[0]);
            Header2 = temp;
            //Header2.OrderBy()
           
        }

        public void sortByDate()
        {
            String[,] temp = Header2.OrderBy(x => x[3]);
            Header2 = temp;
        }

        public void sortBySize()
        {
            String[,] temp = Header2.OrderBy(x => x[2]);
            Header2 = temp;
        }

        public void LinkFile(String link,String origin)
        {


            for(int i = 0; i < 1000; i++)
            {
                if (Header2[i, 0] != null)
                {
                    if (Header2[i, 0].Equals(link) && Header2[i, 5].Equals("not deleted"))
                    {
                        Console.WriteLine("file already exist");
                        return;
                    }
                }
            }
            for (int i = 0; i < 1000; i++)
            {
                if (Header2[i, 0] != null)
                {
                    if (Header2[i, 0].Equals(origin))
                    {
                        /*FileObject linkFile = new FileObject(link, "link");
                        linkFile.setLength(Header[i].getLength());
                        linkFile.setLocation(Header[i].getLocation());
                        Header[HeaderNextIndex] = linkFile;
                        HeaderNextIndex++;*/
                        Byte[] newFileBytes = File.ReadAllBytes(origin);
                        //String S = Convert.ToBase64String(newFileBytes);

                        Header2[HeaderNextIndex, 0] = link ;
                        Header2[HeaderNextIndex, 1] = Header2[i, 1];
                        Header2[HeaderNextIndex, 2] = newFileBytes.Length.ToString();
                        FileInfo fi = new FileInfo(origin);
                        Header2[HeaderNextIndex, 3] = fi.CreationTime.ToString();
                        Header2[HeaderNextIndex, 4] = "link";
                        Header2[HeaderNextIndex, 5] = "not deleted";
                        HeaderNextIndex++;
                        return;
                        
                    }

                }
            }
            Console.WriteLine("file does not exist");
        }



        public string checkMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                   
                    var hash=md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        




        public String getName()
        {
            return this.Name;
        }

        public void convertToFile()
        {
            File.Delete(Name);
            File.Create(Name);


        }

        public Byte[] serialize()
        {
            this.Name = "dkcks";
            String output = JsonConvert.SerializeObject(this);
            Console.WriteLine(this.ContentNextIndex);

            Console.WriteLine(output);
            Byte[] byteOutput = Convert.FromBase64String(output);
            return byteOutput;
        }

        public FileSystem deSerialize(Byte[] input)
        {
            String output = Convert.ToBase64String(input);
            FileSystem origin = JsonConvert.DeserializeObject<FileSystem>(output);
            return origin;
        }

        private void cleanHeader()
        {
            string[,] newHeader2 = new string[1000, 6];
            int newNextIndex = 0;
            for(int i = 0; i < 1000; i++)
            {
                if (Header2[i, 0] != null)
                {
                    if(Header2[i,5].Equals("not deleted"))
                    {
                        newHeader2[newNextIndex, 0] = Header2[i, 0];
                        newHeader2[newNextIndex, 1] = Header2[i, 1];
                        newHeader2[newNextIndex, 2] = Header2[i, 2];
                        newHeader2[newNextIndex, 3] = Header2[i, 3];
                        newHeader2[newNextIndex, 4] = Header2[i, 4];
                        newHeader2[newNextIndex, 5] = Header2[i, 5];
                        newNextIndex++;
                    }
                }
            }
            HeaderNextIndex = newNextIndex;
            Header2 = newHeader2;
        }

        private void clearContent()
        {
            Byte[] newContent = new Byte[100000000];
            int newContentNextIndex = 0;
            for(int i = 0; i < 1000; i++)
            {
                if (Header2[i, 0] != null)
                {
                    int start = int.Parse(Header2[i, 1]);
                    int end = int.Parse(Header2[i, 2])+start;
                    Header2[i,1] = newContentNextIndex.ToString();

                    for(int j = start; j < end; j++)
                    {
                        //Console.WriteLine(newContentNextIndex);
                        newContent[newContentNextIndex] = Content[j];
                        newContentNextIndex++;
                    }
                }
            }
            Content = newContent;
            ContentNextIndex = newContentNextIndex;
        }

    }



    }

