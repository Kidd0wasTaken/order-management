import { useState } from 'react';
import { Alert, Stack } from '@mui/material';
import { useDispatch, useSelector } from 'react-redux';
import { d100Labels } from '../../constants/d100Labels';
import {
  clearD100Error,
  generatePdf,
  generateXml,
  previewD100,
} from '../../store/d100Slice';
import CompanyProfileForm from './CompanyProfileForm';
import DeclarationActions from './DeclarationActions';
import PeriodSelector from './PeriodSelector';
import SalesPreviewCard from './SalesPreviewCard';

export default function D100Page() {
  const dispatch = useDispatch();
  const now = new Date();
  const [luna, setLuna] = useState(now.getMonth() + 1);
  const [an, setAn] = useState(now.getFullYear());

  const {
    preview,
    declaration,
    loadingPreview,
    generatingXml,
    generatingPdf,
    error,
    mutationError,
  } = useSelector((state) => state.d100);

  const period = { luna, an };

  const handlePreview = () => {
    dispatch(clearD100Error());
    dispatch(previewD100(period));
  };

  const handleGenerateXml = () => {
    dispatch(clearD100Error());
    dispatch(generateXml(period));
  };

  const handleGeneratePdf = () => {
    if (!declaration?.id) return;
    dispatch(clearD100Error());
    dispatch(generatePdf(declaration.id));
  };

  return (
    <Stack spacing={3}>
      <Alert severity="info">{d100Labels.disclaimer}</Alert>

      {(error || mutationError) && (
        <Alert severity="error" onClose={() => dispatch(clearD100Error())}>
          {typeof mutationError === 'string' ? mutationError : error}
        </Alert>
      )}

      <CompanyProfileForm />

      <PeriodSelector
        luna={luna}
        an={an}
        onLunaChange={setLuna}
        onAnChange={setAn}
        disabled={loadingPreview || generatingXml || generatingPdf}
      />

      <SalesPreviewCard preview={preview} loading={loadingPreview} />

      <DeclarationActions
        declaration={declaration}
        onPreview={handlePreview}
        onGenerateXml={handleGenerateXml}
        onGeneratePdf={handleGeneratePdf}
        loadingPreview={loadingPreview}
        generatingXml={generatingXml}
        generatingPdf={generatingPdf}
      />
    </Stack>
  );
}
