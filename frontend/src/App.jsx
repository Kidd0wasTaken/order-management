import { useState } from 'react';
import {
  AppBar,
  Box,
  Container,
  Tab,
  Tabs,
  Toolbar,
  Typography,
} from '@mui/material';
import { labels } from './constants/labels';
import { d100Labels } from './constants/d100Labels';
import D100Page from './components/d100/D100Page';
import OrdersPage from './components/OrdersPage';

export default function App() {
  const [tab, setTab] = useState(0);

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'background.default' }}>
      <AppBar position="static" elevation={0} sx={{ borderBottom: '1px solid rgba(255,255,255,0.1)' }}>
        <Toolbar>
          <Typography variant="h6" component="h1" sx={{ flexGrow: 1, fontWeight: 700 }}>
            {labels.appTitle}
          </Typography>
        </Toolbar>
        <Tabs
          value={tab}
          onChange={(_, value) => setTab(value)}
          textColor="inherit"
          indicatorColor="secondary"
          sx={{ px: 2 }}
        >
          <Tab label={labels.ordersTab ?? 'Comenzi'} />
          <Tab label={d100Labels.tabTitle} />
        </Tabs>
      </AppBar>

      <Container maxWidth="xl" sx={{ py: 4 }}>
        {tab === 0 && <OrdersPage />}
        {tab === 1 && <D100Page />}
      </Container>
    </Box>
  );
}
