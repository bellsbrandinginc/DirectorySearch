Imports System.Globalization

Public Class CollectionToBoolConverter
    Implements IValueConverter

    Public Property Converter1 As IValueConverter
    Public Property Converter2 As IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object
        Dim convertedValue As Object = Converter1.Convert(value, targetType, parameter, culture)
        Return Converter2.Convert(convertedValue, targetType, parameter, culture)
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object
        Throw New NotImplementedException()
    End Function

    Private Function IValueConverter_Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Throw New NotImplementedException()
    End Function

    Private Function IValueConverter_ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class