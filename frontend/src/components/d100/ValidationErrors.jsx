import { Alert, List, ListItem, ListItemText } from '@mui/material';

export default function ValidationErrors({ errorsJson }) {
  if (!errorsJson) return null;

  let errors = [];
  try {
    const parsed = JSON.parse(errorsJson);
    errors = Array.isArray(parsed) ? parsed : [String(parsed)];
  } catch {
    errors = [errorsJson];
  }

  if (errors.length === 0) return null;

  return (
    <Alert severity="error">
      <List dense disablePadding>
        {errors.map((err, index) => (
          <ListItem key={index} disableGutters>
            <ListItemText primary={typeof err === 'string' ? err : JSON.stringify(err)} />
          </ListItem>
        ))}
      </List>
    </Alert>
  );
}
