import { FormControl, Grid, InputLabel, MenuItem, Paper, Select, TextField, Typography } from '@mui/material';
import { d100Labels } from '../../constants/d100Labels';

export default function PeriodSelector({ luna, an, onLunaChange, onAnChange, disabled }) {
  const currentYear = new Date().getFullYear();
  const years = Array.from({ length: 6 }, (_, i) => currentYear - 2 + i);

  return (
    <Paper variant="outlined" sx={{ p: 3 }}>
      <Typography variant="h6" gutterBottom>
        {d100Labels.periodSection}
      </Typography>
      <Grid container spacing={2}>
        <Grid item xs={12} sm={6}>
          <FormControl fullWidth disabled={disabled}>
            <InputLabel id="d100-luna-label">{d100Labels.luna}</InputLabel>
            <Select
              labelId="d100-luna-label"
              label={d100Labels.luna}
              value={luna}
              onChange={(e) => onLunaChange(Number(e.target.value))}
            >
              {d100Labels.months.map((name, index) => (
                <MenuItem key={name} value={index + 1}>
                  {name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>
        <Grid item xs={12} sm={6}>
          <TextField
            select
            label={d100Labels.an}
            value={an}
            onChange={(e) => onAnChange(Number(e.target.value))}
            fullWidth
            disabled={disabled}
          >
            {years.map((year) => (
              <MenuItem key={year} value={year}>
                {year}
              </MenuItem>
            ))}
          </TextField>
        </Grid>
      </Grid>
    </Paper>
  );
}
