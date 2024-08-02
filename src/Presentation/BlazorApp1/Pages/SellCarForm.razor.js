export function getFilepondInputFiles (){
  const tokenInput = document.querySelectorAll("input[name='ImageFile']");
  return Array.from(tokenInput, e => e.defaultValue);
}

export function navigate(url)
{
  window.location.replace(url);
}
