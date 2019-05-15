Imports System.IO


Public Class Form1
    Private foldername As String
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
    Private Sub PictureBox1_DragDrop(sender As Object, e As DragEventArgs) Handles PictureBox1.DragDrop
       Dim test As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        Dim fbd As New FolderBrowserDialog()
        TextBox1.Text = test.ToString
        Dim result As DialogResult = fbd.ShowDialog()
        If result = System.Windows.Forms.DialogResult.OK Then
            foldername = fbd.SelectedPath
            TextBox1.Text = fbd.SelectedPath
            BackgroundWorker1.RunWorkerAsync(test)
        Else
            Return
        End If
    End Sub

    Private Sub PictureBox1_DragEnter(sender As Object, e As DragEventArgs) Handles PictureBox1.DragEnter
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork

        Dim test As String() = DirectCast(e.Argument, String())
        For Each file__1 As String In test
            Dim fileinf As New FileInfo(file__1)
            Dim size As Long = fileinf.Length
            If size > 4294967295UI Then
                Label2.Invoke(New Action(Function()
                                             Label2.Text = fileinf.Name
                                             Label2.Show()
                                         End Function))
                Dim chunkSize As Long = 4294967294UI
                Const BUFFER_SIZE As UInteger = 20 * 1024
                Dim buffer As Byte() = New Byte(BUFFER_SIZE - 1) {}

                Using input As System.IO.Stream = File.OpenRead(file__1)
                    Dim index As UInteger = 0
                    While input.Position < input.Length
                        Using output As System.IO.Stream = File.Create((foldername & Convert.ToString("\")) + fileinf.Name & ".6660" & index)
                            Dim totalbytesread As Long = 0
                            Dim remaining As Long = chunkSize, bytesRead As Long
                            While remaining > 0 AndAlso (InlineAssignHelper(bytesRead, input.Read(buffer, 0, Convert.ToInt32(Math.Min(remaining, BUFFER_SIZE))))) > 0
                                output.Write(buffer, 0, Convert.ToInt32(bytesRead))
                                remaining -= bytesRead
                                totalbytesread += bytesRead
                                BackgroundWorker1.ReportProgress(CInt(CDbl(input.Position) / size * 100))
                            End While

                            index += 1

                        End Using
                    End While
                End Using
            Else
                MessageBox.Show(fileinf.Name & vbCrLf & "No need to split less than 4GB file", "PS3 File-Splitter", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Next
    End Sub


    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        ProgressBar1.Value = e.ProgressPercentage
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        ProgressBar1.Value = 0
        Label2.Text = ""
        MessageBox.Show("Split Process Completed", "PS3 File-Splitter", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.AllowDrop = True
        TextBox1.Hide()
        Label2.Hide()
    End Sub
End Class
