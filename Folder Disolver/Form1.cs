using System;
using System.Drawing;
using System.Windows.Forms;
using NovaNet.Utils;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Folder_Disolver
{
    public partial class Form1 : Form
    {
        ListViewItem lvi;
        private Dictionary<string, ListViewItem> ListViewItems = new Dictionary<string, ListViewItem>();
        private Dictionary<string, ListViewItem> ListViewItems1 = new Dictionary<string, ListViewItem>();

        string[] imageName;
        //string imageName1;
        string filespath;

        public static string foldername;
        public Imagery img;
        public string filename;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            List<string> fileNames = new List<string>();
            List<string> tempPath = new System.Collections.Generic.List<string>(1000);
            DialogResult dr = folderBrowserDialog1.ShowDialog();

            toolStripStatusLabel1.Text = "Select Specific Folder...";

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog1.SelectedPath;

                DirectoryInfo selectedPath = new DirectoryInfo(txtPath.Text);
                ListViewItems.Clear();
                ListViewItems1.Clear();
                lstImage.Items.Clear();

                if (selectedPath.GetDirectories().Length > 0)
                {
                    string[] folders = Directory.GetDirectories(txtPath.Text);

                    for (int i = 0; i < folders.Length; i++)
                    {
                        //MessageBox.Show(folders[i].ToString());
                        lstImage.Items.Add(System.IO.Path.GetFileName(folders[i].ToString()));
                        ListViewItems.Add(folders[i].ToString(), lvi);
                    }
                }

                /* foreach (FileInfo file in selectedPath.GetFiles())
                 {
                     if (file.Extension.Equals(".jpg") || file.Extension.Equals(".JPG") || (file.Extension.Equals(".JPEG")) || (file.Extension.Equals(".jpeg")) || file.Extension.Equals(".tif") || file.Extension.Equals(".TIF") || file.Extension.Equals(".pdf"))
                     {
                         fileNames.Add(file.FullName);
                         tempPath.Add(txtPath.Text + "\\" + file.ToString());

                     }
                 }*/
                /*ListViewItems.Clear();
                ListViewItems1.Clear();
                lstImage.Items.Clear();*/

                foldername = selectedPath.Name;

                if (lstImage.Items.Count > 0)
                {
                    btnPDF.Enabled = true;
                    //btnMerge.Enabled = true;
                    toolStripStatusLabel1.Text = "Folder is selected successfully...PDFs are ready to move \t";
                    toolStripProgressBar1.Visible = false;
                }

            }
            else
            {
                btnPDF.Enabled = false;
                toolStripStatusLabel1.Visible = false;
                toolStripProgressBar1.Visible = false;
                toolStripStatusLabel1.Text = "";
                return;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            toolStripStatusLabel1.Visible = true;
            toolStripStatusLabel1.Text = "Select Specific Folder...";
            toolStripProgressBar1.Visible = false;
            txtPath.Enabled = false;
            btnPDF.Enabled = false;

            btnBrowse.Select();
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            List<string> fileNames = new List<string>();
            List<string> tempPath = new System.Collections.Generic.List<string>(1000);
            try
            {
                toolStripStatusLabel1.Text = "PDFs are moving... \t";

                btnBrowse.Enabled = false;
                btnPDF.Enabled = false;


                imageName = null;


                string expFolder = "C:\\";
                bool isDeleted = false;

                

                if (lstImage.Items.Count > 0)
                {
                    ListViewItems.Clear();
                    ListViewItems1.Clear();
                    listBox1.Items.Clear();

                    toolStripStatusLabel1.Text = "PDFs are moving... \t";

                    string executablePath = Path.GetDirectoryName(Application.ExecutablePath);
                    if(!File.Exists(executablePath+"\\log.txt"))
                    {
                        File.Create(executablePath + "\\log.txt");
                    }
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                                          
                    for (int a = 0; a < lstImage.Items.Count; a++)
                    {
                        string filename = lstImage.Items[a].ToString();
                        //imageName[a] = dsexport.Tables[0].Rows[x][4].ToString() + "\\QC" + "\\" + dsimage.Tables[0].Rows[a]["page_name"].ToString();

                        //imageName[a] = txtPath.Text + "\\" + filename.ToString();
                        filespath = txtPath.Text + "\\" + filename.ToString();
                        DirectoryInfo selectedPath = new DirectoryInfo(filespath);

                        
                        foreach (FileInfo file in selectedPath.GetFiles())
                        {
                            if (file.Extension.Equals(".pdf"))
                            {
                                //fileNames.Add(file.FullName);
                                tempPath.Add(filespath + "\\" + file.ToString());

                                //listBox1.Items.Add(System.IO.Path.GetFileName(file.FullName));
                                //ListViewItems1.Add(file.FullName, lvi);
                                listBox1.Items.Add(System.IO.Path.GetFileName(file.FullName));
                                //lstImage.Items.Add(fileName);
                                //ListViewItem lvi1 = lstTotalImage.Items.Add(System.IO.Path.GetFileNameWithoutExtension(fileName));
                                //lvi.Tag = fileName;
                                //lvi1.Tag = fileName;
                                ListViewItems.Add(file.FullName, lvi);
                            }

                        }
                        imageName = new string[listBox1.Items.Count];
                        for (int b = 0; b < listBox1.Items.Count; b++)
                        {
                            imageName[b] = filespath + "\\" + listBox1.Items[b].ToString();

                        }
                        if (imageName.Length != 0)
                        {
                            expFolder = "C:\\";
                            toolStripStatusLabel1.Text = "PDFs are moving... \t";
                            if (Directory.Exists(expFolder + "\\Dissolve\\" + foldername ) && isDeleted == false)
                            {
                                Directory.Delete(expFolder + "\\Dissolve\\" + foldername, true);

                            }

                            if (!Directory.Exists(expFolder + "\\Dissolve\\" + foldername))
                            {
                                Directory.CreateDirectory(expFolder + "\\Dissolve\\" + foldername);
                                isDeleted = true;
                            }
                            
                            expFolder = "C:\\";

                            if (listBox1.Items.Count > 0)
                            {
                                toolStripStatusLabel1.Text = "PDFs are moving... \t";
                                string sourceFile;
                                string destinationFile = expFolder + "\\Dissolve\\" + foldername; 
                                for (int c = 0; c < listBox1.Items.Count; c++)
                                {
                                    

                                    sourceFile = filespath + "\\" + listBox1.Items[c].ToString();
                                    try
                                    {
                                        File.Copy(sourceFile, destinationFile + "\\" + listBox1.Items[c].ToString());
                                    }
                                    catch(Exception ex)
                                    {
                                        
                                        sb.Append(DateTime.Now.ToString()+"  -----Duplicate File-----  " + sourceFile);
                                        sb.AppendLine();
                                       
                                        ex.Message.ToString();
                                    }
                                   
                                }
                               
                            }

                            ListViewItems.Clear();
                            ListViewItems1.Clear();
                            listBox1.Items.Clear();


                        }
                    }
                    toolStripProgressBar1.Visible = true;
                    toolStripProgressBar1.Value = 100;
                    toolStripStatusLabel1.Visible = true;
                    toolStripStatusLabel1.Text = "PDFs moved Successfully... \t";

                    sb.AppendLine();
                    sb.Append("-----------------------------------------------------------------------------------------------");  
                    sb.AppendLine();
                    File.AppendAllText(executablePath + "\\log.txt", sb.ToString());
                    sb.Clear();
                }
                btnBrowse.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
