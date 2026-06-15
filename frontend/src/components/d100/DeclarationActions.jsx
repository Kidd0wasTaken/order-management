import { Button, Paper, Stack, Typography } from '@mui/material';
import { d100Labels } from '../../constants/d100Labels';
import { getPdfDownloadUrl, getXmlDownloadUrl } from '../../api/d100Api';
import ValidationErrors from './ValidationErrors';

export default function DeclarationActions({
  declaration,
  onPreview,
  onGenerateXml,
  onGeneratePdf,
  loadingPreview,
  generatingXml,
  generatingPdf,
}) {
  const canGeneratePdf = declaration?.hasXml && declaration?.id;
  const canDownloadXml = declaration?.hasXml;
  const canDownloadPdf = declaration?.hasPdf;

  return (
    <Paper variant="outlined" sx={{ p: 3 }}>
      <Typography variant="h6" gutterBottom>
        {d100Labels.actionsSection}
      </Typography>

      {declaration && (
        <>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
            {d100Labels.status}: {declaration.status}
          </Typography>
          <ValidationErrors errorsJson={declaration.validationErrors} />
        </>
      )}

      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1} sx={{ mt: 2 }} flexWrap="wrap" useFlexGap>
        <Button variant="outlined" onClick={onPreview} disabled={loadingPreview}>
          {loadingPreview ? d100Labels.loading : d100Labels.preview}
        </Button>
        <Button variant="contained" onClick={onGenerateXml} disabled={generatingXml}>
          {generatingXml ? d100Labels.generating : d100Labels.generateXml}
        </Button>
        <Button
          variant="contained"
          color="secondary"
          onClick={onGeneratePdf}
          disabled={!canGeneratePdf || generatingPdf}
        >
          {generatingPdf ? d100Labels.generating : d100Labels.generatePdf}
        </Button>
        {canDownloadXml && (
          <Button component="a" href={getXmlDownloadUrl(declaration.id)} download variant="outlined">
            {d100Labels.downloadXml}
          </Button>
        )}
        {canDownloadPdf && (
          <Button component="a" href={getPdfDownloadUrl(declaration.id)} download variant="outlined">
            {d100Labels.downloadPdf}
          </Button>
        )}
      </Stack>
    </Paper>
  );
}
