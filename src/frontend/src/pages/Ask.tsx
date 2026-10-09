import { useState } from 'react';
import { Alert, Box, Button, Chip, CircularProgress, Paper, Stack, TextField, Typography } from '@mui/material';
import { ask, type AskResponse } from '../services/askService';

const Ask = () => {
  const [question, setQuestion] = useState('');
  const [result, setResult] = useState<AskResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const submit = async () => {
    if (!question.trim()) return;
    setLoading(true);
    setError(null);
    try {
      setResult(await ask(question));
    } catch (e) {
      setResult(null);
      setError(e instanceof Error ? e.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ maxWidth: 800 }}>
      <Typography variant="h5" gutterBottom>Ask about your projects</Typography>
      <Stack direction="row" spacing={2} sx={{ mb: 3 }}>
        <TextField
          fullWidth
          label="Question"
          value={question}
          onChange={(e) => setQuestion(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && submit()}
        />
        <Button variant="contained" onClick={submit} disabled={loading}>Ask</Button>
      </Stack>

      {loading && <CircularProgress />}
      {error && <Alert severity="error">{error}</Alert>}

      {result && (
        <Paper sx={{ p: 2 }}>
          {result.abstained && <Alert severity="info" sx={{ mb: 2 }}>No answer found in the documents.</Alert>}
          <Typography sx={{ whiteSpace: 'pre-wrap' }}>{result.answer}</Typography>
          {result.sources.length > 0 && (
            <Stack direction="row" spacing={1} sx={{ mt: 2 }} flexWrap="wrap" useFlexGap>
              {result.sources.map((s) => <Chip key={s} label={s} size="small" />)}
            </Stack>
          )}
        </Paper>
      )}
    </Box>
  );
};

export default Ask;
