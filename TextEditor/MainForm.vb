'' Program name:        Text Editor
'' Author:              Simon Yarrow (100738683)
'' Course:              NETD 2202
'' Date:                July 25, 2019
'' Program purpose:     This Windows form is a simple text editor program. A user can type text into a blank text box or load an existing
''                      text file and edit it (with Open), then save their work using Save or Save As (the latter will prompt for a file
''                      name to create or overwrite, while the former will overwrite the existing file if there is one or prompt for a file
''                      name otherwise). Users can Cut, Copy, and Paste text as normal for a Windows program. All of these options (as well
''                      as a simple pop-up window with information about the program) can be accessed using the mouse from drop-down menus
''                      or using keyboard shortcuts with the Alt key, and some of them (as noted in the menus) can also be accessed more
''                      immediately with keyboard shurtcuts using the Ctrl key. A pop-up window will alert the user if something they typed
''                      is about to be discarded by an action they are taking.

Option Strict On
Imports System.IO       '' For reading and writing files
Public Class MainForm

    ''CONSTANTS AND VARIABLES (class-level)
    Const FormNameDefault As String = "Text Editor"     '' The default text at the top of the window
    Dim latestFileName As String = ""                   '' The file name of the current file (non-empty only if it was opened or saved)

    ''' <summary>
    ''' Private subroutine "CheckSave" (no parameters) - If no file has been opened or saved, but text has been entered into the text box
    ''' (i.e., into a new file), alert the user that their text will be discarded. For all other situations EXCEPT a blank text box when
    ''' no file has been opened or saved, check the text box contents against the contents of the file that was opened or saved, and if
    ''' they differ, alert the user that their changes will be discarded (catch an exception and give another error message if there was
    ''' a problem accessing the file that was opened or saved).
    ''' </summary>
    Private Sub CheckSave()
        Dim loadedFile As String = ""       '' This will hold the contents of the file that was opened or saved (if applicable)

        '' If the user has entered text (other than white space) into a new file, alert them that the text will be discarded
        If (Trim(tbMainBox.Text) IsNot "") AndAlso (latestFileName Is "") Then
            MessageBox.Show("Your text will be discarded!")
            '' Otherwise, for any situation other than empty text in a new file...
        ElseIf Not (Trim(tbMainBox.Text) Is "") AndAlso (latestFileName Is "") Then
            '' Try...
            Try
                '' Load the contents of the file that was opened or saved into a StreamReader
                Dim reader As New StreamReader(latestFileName)
                '' Use the StreamReader to save those contents to the string variable
                loadedFile = reader.ReadToEnd
                '' Close the StreamReader to release the resource
                reader.Close()
                '' If the contents of the file do not match the contents of the text box, alert user that the changes will be discarded
                If Not (loadedFile = tbMainBox.Text) Then
                    MessageBox.Show("Changes will be discarded!")
                End If
                '' If attempting to access the file that was saved or opened causes an exception, give an error message without
                '' closing the program
            Catch ex As Exception
                Throw New ApplicationException(ex.ToString)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Subroutine to handle clicking Exit in the File drop-down menu - Call CheckSave subroutine to give alert if text is being discarded,
    ''' then close the entire application.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub mnuExit_Click(sender As Object, e As EventArgs) Handles mnuExit.Click
        CheckSave()
        Application.Exit()
    End Sub

    ''' <summary>
    ''' Subroutine to handle clicking Open in the File drop-down menu - If user clicks OK in the OpenFileDialog that pops up, call CheckSave
    ''' subroutine to give alert if text is being discarded, then try to load the contents of the selected file into the text box and append
    ''' the file name to the text (the program's name) at the top of the window. If an exception is thrown trying to access the file, give an
    ''' error message without closing the program.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub mnuOpen_Click(sender As Object, e As EventArgs) Handles mnuOpen.Click
        '' Do the following if the user clicks OK in the Open File Dialog that pops up
        If OpenFileDialog.ShowDialog() = DialogResult.OK Then
            '' Give alert if changes will be discarded
            CheckSave()
            '' Try to open the file, read its contents, and set the text box contents to the file's contents, but catch an exception if there
            '' is a problem opening or reading the file, and if so give an error message without closing the program
            Try
                Dim reader As New StreamReader(OpenFileDialog.FileName)
                tbMainBox.Text = reader.ReadToEnd
                reader.Close()                                      '' Release the resource
                latestFileName = OpenFileDialog.FileName            '' Save the file name
                Me.Text = FormNameDefault + " " + latestFileName    '' Add the file name to the program name at the top of the window
            Catch ex As Exception
                Throw New ApplicationException(ex.ToString)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Subroutine to handle clicking Save As in the File drop-down menu - In the Save File Dialog that pops up, only allow viewing and
    ''' saving of text files (this program can only handle text files), and if the user clicks OK in the dialog, save the file name they
    ''' entered, change the text at the top of the window to reflect that file name, then run the Save subroutine.
    ''' </summary>
    Private Sub mnuSaveAs_Click() Handles mnuSaveAs.Click
        SaveFileDialog.Filter = "TXT Files (*.txt)|*.txt"
        If SaveFileDialog.ShowDialog() = DialogResult.OK Then
            latestFileName = SaveFileDialog.FileName
            Me.Text = FormNameDefault + " " + SaveFileDialog.FileName
            mnuSave_Click()
        End If
    End Sub

    ''' <summary>
    ''' Subroutine to handle clicking Save in the File drop-down menu - If the current text box contents do not have an associated file name
    ''' (because they have not been opened or saved), run the Save As subroutine, otherwise write all text in the text box to the file name
    ''' (do not append to the end of that file).
    ''' </summary>
    Private Sub mnuSave_Click() Handles mnuSave.Click
        If latestFileName = "" Then
            mnuSaveAs_Click()
        Else
            My.Computer.FileSystem.WriteAllText(latestFileName, tbMainBox.Text, False)
        End If
    End Sub

    ''' <summary>
    ''' Subroutine to handle clicking New in the File drop-down menu - Call the CheckSave subroutine to give an alert if any text is going
    ''' to be discarded, then clear the text box, clear the variable storing the name of the latest file that was opened or saved, and
    ''' reset the text at the top of the window (to the program's name).
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub mnuNew_Click(sender As Object, e As EventArgs) Handles mnuNew.Click
        CheckSave()
        tbMainBox.Clear()
        latestFileName = ""
        Me.Text = FormNameDefault
    End Sub

    ''' <summary>
    ''' Subroutine to handle clicking Copy in the Edit drop-down menu - Copy any selected text in the text box to the Windows clipboard.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub mnuCopy_Click(sender As Object, e As EventArgs) Handles mnuCopy.Click
        tbMainBox.Copy()
    End Sub

    ''' <summary>
    ''' Subroutine to handle clicking Cut in the Edit drop-down menu - Delete any selected text in the text box and add it to
    ''' the Windows clipboard.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub mnuCut_Click(sender As Object, e As EventArgs) Handles mnuCut.Click
        tbMainBox.Cut()
    End Sub

    ''' <summary>
    ''' Subroutine to handle clicking Paste in the Edit drop-down menu - Paste the contents of the Windows clipboard at the current
    ''' cursor location in the text box (this will overwrite any selected text in the text box).
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub mnuPaste_Click(sender As Object, e As EventArgs) Handles mnuPaste.Click
        tbMainBox.Paste()
    End Sub

    ''' <summary>
    ''' Subroutine to handle clicking About in the Help drop-down menu - Pop up a message box displaying the course, assignment number,
    ''' and author of this program.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub mnuAbout_Click(sender As Object, e As EventArgs) Handles mnuAbout.Click
        MessageBox.Show("NETD-2202" + vbCrLf + "Lab #5" + vbCrLf + "S. Yarrow")
    End Sub
End Class
