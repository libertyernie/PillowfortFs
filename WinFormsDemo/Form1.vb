Imports System.Net
Imports PillowfortFs

Public Class Form1
    Private _client As PillowfortClient

    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        _client = Await PillowfortClientFactory.CreateClientAsync(TextBox1.Text, TextBox2.Text)
        Label1.Text = Await _client.WhoamiAsync()

        Dim avatarUrl = Await _client.GetAvatarAsync()
        Dim req = WebRequest.CreateHttp(avatarUrl)
        Using resp = Await req.GetResponseAsync()
            Using stream = resp.GetResponseStream()
                Dim i = Image.FromStream(stream)
                PictureBox1.Image = i
            End Using
        End Using
    End Sub

    Private Async Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If _client IsNot Nothing Then
            Await _client.SignoutAsync()
        End If
        _client = Nothing
        Label1.Text = "Logged out"
        PictureBox1.Image = Nothing
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If _client IsNot Nothing Then
            _client.Signout()
        End If
    End Sub
End Class
