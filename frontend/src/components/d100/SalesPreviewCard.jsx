import { Box, CircularProgress, Paper, Stack, Typography } from '@mui/material';
import { d100Labels } from '../../constants/d100Labels';
import { formatCurrency } from '../../utils/formatters';

function Row({ label, value }) {
  return (
    <Stack direction="row" justifyContent="space-between" spacing={2}>
      <Typography color="text.secondary">{label}</Typography>
      <Typography fontWeight={500}>{value}</Typography>
    </Stack>
  );
}

export default function SalesPreviewCard({ preview, loading }) {
  return (
    <Paper variant="outlined" sx={{ p: 3 }}>
      <Typography variant="h6" gutterBottom>
        {d100Labels.previewSection}
      </Typography>

      {loading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
          <CircularProgress size={32} />
        </Box>
      )}

      {!loading && !preview && (
        <Typography color="text.secondary">{d100Labels.noPreview}</Typography>
      )}

      {!loading && preview && (
        <Stack spacing={1.5}>
          <Row label={d100Labels.orderCount} value={preview.orderCount} />
          <Row label={d100Labels.totalSales} value={formatCurrency(preview.totalSales)} />
          <Row label={d100Labels.sumaDat} value={formatCurrency(preview.sumaDat)} />
          <Row label={d100Labels.totalPlataA} value={formatCurrency(preview.totalPlataA)} />
          <Row label={d100Labels.codOblig} value={preview.obligationLine?.codOblig ?? '—'} />
          <Row label={d100Labels.codBugetar} value={preview.obligationLine?.codBugetar ?? '—'} />
          <Row label={d100Labels.scadenta} value={preview.obligationLine?.scadenta ?? '—'} />
          <Typography variant="body2" color="warning.main" sx={{ mt: 1 }}>
            {preview.simulationNote || d100Labels.disclaimer}
          </Typography>
        </Stack>
      )}
    </Paper>
  );
}
