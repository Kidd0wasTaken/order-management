import { useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  Grid,
  Paper,
  TextField,
  Typography,
} from '@mui/material';
import { useDispatch, useSelector } from 'react-redux';
import { d100Labels } from '../../constants/d100Labels';
import { clearD100Error, loadCompany, saveCompany } from '../../store/d100Slice';

const emptyForm = {
  cui: '',
  denumire: '',
  adresa: '',
  telefon: '',
  email: '',
  numeDeclar: '',
  prenumeDeclar: '',
  functieDeclar: '',
};

function toForm(company) {
  if (!company) return emptyForm;
  return {
    cui: company.cui ?? '',
    denumire: company.denumire ?? '',
    adresa: company.adresa ?? '',
    telefon: company.telefon ?? '',
    email: company.email ?? '',
    numeDeclar: company.numeDeclar ?? '',
    prenumeDeclar: company.prenumeDeclar ?? '',
    functieDeclar: company.functieDeclar ?? '',
  };
}

export default function CompanyProfileForm() {
  const dispatch = useDispatch();
  const { company, loadingCompany, savingCompany, mutationError } = useSelector((state) => state.d100);
  const [form, setForm] = useState(emptyForm);

  useEffect(() => {
    dispatch(loadCompany());
  }, [dispatch]);

  useEffect(() => {
    setForm(toForm(company));
  }, [company]);

  const handleChange = (field) => (event) => {
    setForm((prev) => ({ ...prev, [field]: event.target.value }));
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    dispatch(saveCompany(form));
  };

  return (
    <Paper variant="outlined" sx={{ p: 3 }}>
      <Typography variant="h6" gutterBottom>
        {d100Labels.companySection}
      </Typography>

      {mutationError && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => dispatch(clearD100Error())}>
          {mutationError}
        </Alert>
      )}

      <Box component="form" onSubmit={handleSubmit}>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={4}>
            <TextField
              label={d100Labels.cui}
              value={form.cui}
              onChange={handleChange('cui')}
              required
              fullWidth
              disabled={loadingCompany}
            />
          </Grid>
          <Grid item xs={12} sm={8}>
            <TextField
              label={d100Labels.denumire}
              value={form.denumire}
              onChange={handleChange('denumire')}
              required
              fullWidth
              disabled={loadingCompany}
            />
          </Grid>
          <Grid item xs={12}>
            <TextField
              label={d100Labels.adresa}
              value={form.adresa}
              onChange={handleChange('adresa')}
              required
              fullWidth
              multiline
              minRows={2}
              disabled={loadingCompany}
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              label={d100Labels.telefon}
              value={form.telefon}
              onChange={handleChange('telefon')}
              fullWidth
              disabled={loadingCompany}
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              label={d100Labels.email}
              type="email"
              value={form.email}
              onChange={handleChange('email')}
              fullWidth
              disabled={loadingCompany}
            />
          </Grid>
          <Grid item xs={12} sm={4}>
            <TextField
              label={d100Labels.numeDeclar}
              value={form.numeDeclar}
              onChange={handleChange('numeDeclar')}
              required
              fullWidth
              disabled={loadingCompany}
            />
          </Grid>
          <Grid item xs={12} sm={4}>
            <TextField
              label={d100Labels.prenumeDeclar}
              value={form.prenumeDeclar}
              onChange={handleChange('prenumeDeclar')}
              required
              fullWidth
              disabled={loadingCompany}
            />
          </Grid>
          <Grid item xs={12} sm={4}>
            <TextField
              label={d100Labels.functieDeclar}
              value={form.functieDeclar}
              onChange={handleChange('functieDeclar')}
              required
              fullWidth
              disabled={loadingCompany}
            />
          </Grid>
        </Grid>

        <Box sx={{ mt: 2, display: 'flex', justifyContent: 'flex-end' }}>
          <Button type="submit" variant="contained" disabled={loadingCompany || savingCompany}>
            {savingCompany ? d100Labels.saving : d100Labels.saveCompany}
          </Button>
        </Box>
      </Box>
    </Paper>
  );
}
