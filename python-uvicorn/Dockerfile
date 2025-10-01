FROM python:3.13

# For Manim
RUN apt-get install -y \
  gcc

WORKDIR /python-uvicorn
COPY . .
RUN pip install --no-cache-dir .

EXPOSE 8000

CMD ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8000"]
