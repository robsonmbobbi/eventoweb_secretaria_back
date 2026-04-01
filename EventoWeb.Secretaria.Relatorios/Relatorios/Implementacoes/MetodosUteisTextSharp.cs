namespace EventoWeb.Secretaria.Relatorios.Relatorios.Implementacoes
{
    public static class MetodosUteisTextSharp
    {
        public static float MillimetersToPointsTextSharp(this float value)
        {
            return value / 25.4f * 72f;
        }
    }
}
