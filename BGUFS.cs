using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BGUFS
{
    class BGUFS
    {
        static void Main(string[] args)
        {
            //  FileSystem FO = new FileSystem("Sdsc.dat");
            //String afterSer = serialize(FO);
            //FileSystem FO2 = JsonConvert.DeserializeObject<FileSystem>(afterSer);
            //Console.WriteLine(FO2.getName());


            switch (args[0])//args[0])
                {

                    case "-create":
                        FileSystem fileSystem = new FileSystem(args[1]);
                        String toAdd = serialize(fileSystem);
                        File.WriteAllText(args[1], toAdd);
                        //String read= File.ReadAllText(args[1]);
                        break;




                    case "-add":
                        String toRead = File.ReadAllText(args[1]);
                        FileSystem fileSystem1 = deSerialize(toRead);
                        bool added = fileSystem1.addFile(args[2]);
                        if (!added)
                        {
                            Console.WriteLine("file already exist");
                        }
                        else
                        {
                            String updateFSString1 = serialize(fileSystem1);
                            File.WriteAllText(args[1], updateFSString1);
                         


                        }
                        break;

                 
                    
                    case "-remove":
                        String toRead3 = File.ReadAllText(args[1]);
                        FileSystem fileSystem3 = deSerialize(toRead3);
                        bool removed = fileSystem3.removeFile(args[2]);

                        if (!removed)
                        {
                            Console.WriteLine("file does not exist");
                        }
                        else
                        {
                            String updateFSString4 = serialize(fileSystem3);
                            File.WriteAllText(args[1], updateFSString4);
                     


                        }



                        break;


                case "-rename":
                    String toRead5 = File.ReadAllText(args[1]);
                    FileSystem fileSystem5 = deSerialize(toRead5);
                    fileSystem5.rename(args[2], args[3]);
                    String updateFSString5 = serialize(fileSystem5);
                    File.WriteAllText(args[1], updateFSString5);
                    break;


                case "-extract":
                    String toRead6 = File.ReadAllText(args[1]);
                    FileSystem fileSystem6 = deSerialize(toRead6);
                    fileSystem6.extract(args[2], args[3]);

                    break;

                case "-dir":
                    String toRead7 = File.ReadAllText(args[1]);
                    FileSystem fileSystem7 = deSerialize(toRead7);
                    fileSystem7.dir();
                    break;

                case "-hash":
                    String toRead8 = File.ReadAllText(args[1]);
                    FileSystem fileSystem8 = deSerialize(toRead8);
                    fileSystem8.hash(args[2]);
                    break;

                case "-sortAB":
                    String toRead9 = File.ReadAllText(args[1]);
                    FileSystem fileSystem9 = deSerialize(toRead9);
                    fileSystem9.sortAB();
                    String updateFSString9 = serialize(fileSystem9);
                    File.WriteAllText(args[1], updateFSString9);
                    break;

                case "-sortDate":
                    String toRead10 = File.ReadAllText(args[1]);
                    FileSystem fileSystem10 = deSerialize(toRead10);
                    fileSystem10.sortByDate();
                    String updateFSString10 = serialize(fileSystem10);
                    File.WriteAllText(args[1], updateFSString10);
                    break;

                case "-sortSize":
                    String toRead11 = File.ReadAllText(args[1]);
                    FileSystem fileSystem11 = deSerialize(toRead11);
                    fileSystem11.sortBySize();
                    String updateFSString11 = serialize(fileSystem11);
                    File.WriteAllText(args[1], updateFSString11);
                    break;

                case "-addLink":
                    String toRead12 = File.ReadAllText(args[1]);
                    FileSystem fileSystem12 = deSerialize(toRead12);
                    fileSystem12.LinkFile(args[2],args[3]);
                    String updateFSString12 = serialize(fileSystem12);
                    File.WriteAllText(args[1], updateFSString12);
                    break;

                case "-optimize":
                    String toRead13 = File.ReadAllText(args[1]);
                    FileSystem fileSystem13 = deSerialize(toRead13);
                    fileSystem13.optimize();
                    String updateFSString13 = serialize(fileSystem13);
                    File.WriteAllText(args[1], updateFSString13);
                    break;


            }




            /*case "-add":
                String fileSystemPath = args[1];
                String filePath = args[2];
                FileSystem f = null;
                for (int i = 0; i < FSList.Count; i++)
                {
                    if (FSList.ToArray()[i].getName().Equals(fileSystemPath))
                    {
                        f = FSList[i];
                    }

                }
                if (f != null)
                {
                    bool added = f.addFile(args[2]);
                    if (added)
                    {
                        break;
                    }

                    else
                    {
                        Console.WriteLine("file already exist");
                    }
                }


                break;



*/
        }
            

            public static string serialize(FileSystem F)
        {
            string output = JsonConvert.SerializeObject(F);
            return output;
            //Byte[] byteOutput = Convert.FromBase64String(output);
            //return byteOutput;
        }
        public static FileSystem deSerialize(String input)
        {
            FileSystem origin = JsonConvert.DeserializeObject<FileSystem>(input);
            return origin;
        }


    }

   


    }


