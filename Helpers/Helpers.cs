namespace petmypet.Helpers
{
    public static class Helpers
    {
        private static string RemoverAcentos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return texto;

            var normalized = texto.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();

            foreach (var ch in normalized)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        private static string ApenasDigitos(string texto)
        {
            return new string(texto?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
        }

    }
}
