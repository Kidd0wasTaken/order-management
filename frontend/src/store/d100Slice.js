import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import * as d100Api from '../api/d100Api';
import { getErrorMessage } from '../utils/formatters';

export const loadCompany = createAsyncThunk('d100/loadCompany', async (_, { rejectWithValue }) => {
  try {
    return await d100Api.fetchCompany();
  } catch (error) {
    if (error?.response?.status === 404) return null;
    return rejectWithValue(getErrorMessage(error, 'Eroare la încărcarea profilului companiei'));
  }
});

export const saveCompany = createAsyncThunk('d100/saveCompany', async (dto, { rejectWithValue }) => {
  try {
    return await d100Api.saveCompany(dto);
  } catch (error) {
    return rejectWithValue(getErrorMessage(error, 'Eroare la salvarea profilului'));
  }
});

export const previewD100 = createAsyncThunk('d100/preview', async (period, { rejectWithValue }) => {
  try {
    return await d100Api.previewDeclaration(period);
  } catch (error) {
    return rejectWithValue(getErrorMessage(error, 'Eroare la previzualizare'));
  }
});

export const generateXml = createAsyncThunk('d100/generateXml', async (period, { rejectWithValue }) => {
  try {
    return await d100Api.generateDeclaration(period);
  } catch (error) {
    const data = error?.response?.data;
    return rejectWithValue({
      message: getErrorMessage(error, 'Eroare la generarea XML'),
      errors: data?.errors,
      declaration: data?.declaration,
    });
  }
});

export const generatePdf = createAsyncThunk('d100/generatePdf', async (id, { rejectWithValue }) => {
  try {
    return await d100Api.generatePdf(id);
  } catch (error) {
    return rejectWithValue(getErrorMessage(error, 'Eroare la generarea PDF'));
  }
});

const initialState = {
  company: null,
  preview: null,
  declaration: null,
  loadingCompany: false,
  savingCompany: false,
  loadingPreview: false,
  generatingXml: false,
  generatingPdf: false,
  error: null,
  mutationError: null,
};

const d100Slice = createSlice({
  name: 'd100',
  initialState,
  reducers: {
    clearD100Error(state) {
      state.error = null;
      state.mutationError = null;
    },
    clearPreview(state) {
      state.preview = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loadCompany.pending, (state) => {
        state.loadingCompany = true;
        state.error = null;
      })
      .addCase(loadCompany.fulfilled, (state, action) => {
        state.loadingCompany = false;
        state.company = action.payload;
      })
      .addCase(loadCompany.rejected, (state, action) => {
        state.loadingCompany = false;
        state.error = action.payload;
      })
      .addCase(saveCompany.pending, (state) => {
        state.savingCompany = true;
        state.mutationError = null;
      })
      .addCase(saveCompany.fulfilled, (state, action) => {
        state.savingCompany = false;
        state.company = action.payload;
      })
      .addCase(saveCompany.rejected, (state, action) => {
        state.savingCompany = false;
        state.mutationError = action.payload;
      })
      .addCase(previewD100.pending, (state) => {
        state.loadingPreview = true;
        state.mutationError = null;
      })
      .addCase(previewD100.fulfilled, (state, action) => {
        state.loadingPreview = false;
        state.preview = action.payload;
      })
      .addCase(previewD100.rejected, (state, action) => {
        state.loadingPreview = false;
        state.mutationError = action.payload;
      })
      .addCase(generateXml.pending, (state) => {
        state.generatingXml = true;
        state.mutationError = null;
      })
      .addCase(generateXml.fulfilled, (state, action) => {
        state.generatingXml = false;
        state.declaration = action.payload;
      })
      .addCase(generateXml.rejected, (state, action) => {
        state.generatingXml = false;
        state.mutationError = action.payload?.message ?? action.payload;
        if (action.payload?.declaration) {
          state.declaration = action.payload.declaration;
        }
      })
      .addCase(generatePdf.pending, (state) => {
        state.generatingPdf = true;
        state.mutationError = null;
      })
      .addCase(generatePdf.fulfilled, (state, action) => {
        state.generatingPdf = false;
        state.declaration = action.payload;
      })
      .addCase(generatePdf.rejected, (state, action) => {
        state.generatingPdf = false;
        state.mutationError = action.payload;
      });
  },
});

export const { clearD100Error, clearPreview } = d100Slice.actions;
export default d100Slice.reducer;
